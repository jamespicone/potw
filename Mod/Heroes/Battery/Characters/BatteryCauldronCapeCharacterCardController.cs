using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Battery
{
    public class BatteryCauldronCapeCharacterCardController : BatteryUtilityCharacterCardController
    {
        public BatteryCauldronCapeCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            DischargePowerIndex = 1;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            switch(index)
            {
                case 0:
                    // "Put the top card of your deck into play face down."
                    if (TurnTaker.Deck.IsEmpty)
                    {
                        if (TurnTaker.Trash.IsEmpty)
                        {
                            IEnumerator messageCoroutine = GameController.SendMessageAction(TurnTaker.Name + "'s deck and trash are empty, so they can't put a card into play from their deck.", Priority.Medium, GetCardSource());
                            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(messageCoroutine); }
                            else { GameController.ExhaustCoroutine(messageCoroutine); }
                        }
                        else
                        {
                            IEnumerator shuffleCoroutine = GameController.ShuffleTrashIntoDeck(TurnTakerController, cardSource: GetCardSource());
                            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(shuffleCoroutine); }
                            else { GameController.ExhaustCoroutine(shuffleCoroutine); }
                        }
                    }
                    
                    var top = TurnTaker.Deck.TopCard;
                    if (top == null) { yield break; }

                    if (top.IsMissionCard)
                    {
                        // We know from Santa Guise that playing an OA Mission "face down" from your deck results in it entering play Reward side up.
                        IEnumerator missionCoroutine = GameController.SendMessageAction(CharacterCard.Title + "'s Charge produces a Reward!", Priority.Low, GetCardSource(), new Card[] { top }, true);
                        if (UseUnityCoroutines) { yield return GameController.StartCoroutine(missionCoroutine); }
                        else { GameController.ExhaustCoroutine(missionCoroutine); }
                    }
                    else
                    {
                        IEnumerator prepareCoroutine = GameController.FlipCard(FindCardController(top), cardSource: GetCardSource());
                        if (UseUnityCoroutines) { yield return GameController.StartCoroutine(prepareCoroutine); }
                        else { GameController.ExhaustCoroutine(prepareCoroutine); }
                    }

                    IEnumerator moveCoroutine = GameController.MoveCard(TurnTakerController, top, TurnTaker.PlayArea, playCardIfMovingToPlayArea: top.IsMissionCard, responsibleTurnTaker: TurnTaker, cardSource: GetCardSource());
                    if (UseUnityCoroutines) { yield return GameController.StartCoroutine(moveCoroutine); }
                    else { GameController.ExhaustCoroutine(moveCoroutine); }
                    break;

                case 1:
                    // "{Charge} {BatteryCharacter}..."
                    IEnumerator chargeCoroutine = Charge(Card);
                    if (UseUnityCoroutines) { yield return GameController.StartCoroutine(chargeCoroutine); }
                    else { GameController.ExhaustCoroutine(chargeCoroutine); }

                    // "... until the start of your next turn."
                    OnPhaseChangeStatusEffect chargeExpiration = new OnPhaseChangeStatusEffect(Card, nameof(ChargeExpiresResponse), "At the start of " + base.HeroTurnTakerController.Name + "'s next turn, {Discharge} " + Card.Title + ".", new TriggerType[] { TriggerType.MoveCard }, Card);
                    chargeExpiration.NumberOfUses = 1;
                    chargeExpiration.BeforeOrAfter = BeforeOrAfter.After;
                    chargeExpiration.TurnTakerCriteria.IsSpecificTurnTaker = TurnTaker;
                    chargeExpiration.TurnPhaseCriteria.Phase = Phase.Start;
                    chargeExpiration.TurnPhaseCriteria.TurnTaker = TurnTaker;
                    chargeExpiration.TurnIndexCriteria.GreaterThan = Game.TurnIndex;
                    IEnumerator expireCoroutine = GameController.AddStatusEffect(chargeExpiration, true, GetCardSource());
                    if (UseUnityCoroutines) { yield return GameController.StartCoroutine(expireCoroutine); }
                    else { GameController.ExhaustCoroutine(expireCoroutine); }

                    // Reveal each face-down card in your play area, then put them into play in any order.
                    var faceDownCards = FindCardsWhere(
                        c => c.IsFaceDownNonCharacter && !c.IsMissionCard && c.Location == TurnTaker.PlayArea,
                        realCardsOnly: true,
                        GetCardSource()
                    );

                    var e = GameController.MoveCards(TurnTakerController, faceDownCards, TurnTaker.Revealed, responsibleTurnTaker: TurnTaker, cardSource: GetCardSource());
                    if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                    else { GameController.ExhaustCoroutine(e); }

                    var cardsToPlay = faceDownCards.ToList();
                    
                    while (cardsToPlay.Count() > 0)
                    {
                        var storedResults = new List<SelectCardDecision>();
                        e = GameController.SelectCardAndStoreResults(
                            HeroTurnTakerController,
                            SelectionType.PlayCard,
                            cardsToPlay,
                            storedResults,
                            cardSource: GetCardSource()
                        );
                        if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                        else { GameController.ExhaustCoroutine(e); }

                        var cardToPlay = storedResults.FirstOrDefault()?.SelectedCard;
                        if (cardToPlay == null) { break; }

                        cardsToPlay.Remove(cardToPlay);

                        e = GameController.PlayCard(
                            TurnTakerController,
                            cardToPlay,
                            isPutIntoPlay: true,
                            cardSource: GetCardSource()
                        );
                        if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                        else { GameController.ExhaustCoroutine(e); }
                    }

                    e = CleanupCardsAtLocations(
                        new List<Location> { TurnTaker.Revealed },
                        TurnTaker.Trash,
                        cardsInList: faceDownCards.ToList()
                    );
                    if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                    else { GameController.ExhaustCoroutine(e); }
                    
                    break;
            }
        }

        public IEnumerator ChargeExpiresResponse(PhaseChangeAction pca, OnPhaseChangeStatusEffect effect)
        {
            // It's the start of Battery's next turn, so remove Battery's Charge
            IEnumerator removeCoroutine = RemoveCharge(Card);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(removeCoroutine); }
            else { GameController.ExhaustCoroutine(removeCoroutine); }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator incapCoroutine;
            switch (index)
            {
                case 0:
                    incapCoroutine = UseIncapOption1();
                    break;

                case 1:
                    incapCoroutine = UseIncapOption2();
                    break;

                case 2:
                    incapCoroutine = UseIncapOption3();
                    break;

                default: yield break;
            }

            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(incapCoroutine); }
            else { GameController.ExhaustCoroutine(incapCoroutine); }
        }

        private IEnumerator UseIncapOption1()
        {
            // "One player may draw a card now."
            IEnumerator drawCoroutine = GameController.SelectHeroToDrawCard(HeroTurnTakerController, cardSource: GetCardSource());
            if (UseUnityCoroutines) { yield return base.GameController.StartCoroutine(drawCoroutine); }
            else { GameController.ExhaustCoroutine(drawCoroutine); }
        }

        private IEnumerator UseIncapOption2()
        {
            // "One hero target deals 1 target 2 melee damage."
            List<SelectCardDecision> chooseHeroTarget = new List<SelectCardDecision>();
            IEnumerator chooseHeroCoroutine = GameController.SelectCardAndStoreResults(HeroTurnTakerController, SelectionType.CardToDealDamage, new LinqCardCriteria((Card c) => c.IsInPlay && c.Is(this).Hero().Target(), "hero target", useCardsSuffix: false), chooseHeroTarget, false, cardSource: GetCardSource());
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(chooseHeroCoroutine); }
            else { GameController.ExhaustCoroutine(chooseHeroCoroutine); }

            SelectCardDecision choice = chooseHeroTarget.FirstOrDefault();
            if (choice != null && choice.SelectedCard != null)
            {
                IEnumerator damageCoroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, choice.SelectedCard), 2, DamageType.Melee, 1, false, 1, cardSource: GetCardSource());
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(damageCoroutine); }
                else { GameController.ExhaustCoroutine(damageCoroutine); }
            }
        }

        private IEnumerator UseIncapOption3()
        {
            // "Destroy a non-character card Equipment or Device card."
            IEnumerator destroyCoroutine = GameController.SelectAndDestroyCard(HeroTurnTakerController, new LinqCardCriteria((Card c) => !c.IsCharacter && (c.Is(this).Equipment() || c.IsDevice), "non-character Equipment or Device"), false, responsibleCard: Card, cardSource: GetCardSource());
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(destroyCoroutine); }
            else { GameController.ExhaustCoroutine(destroyCoroutine); }
        }
    }
}
