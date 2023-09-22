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

        private IEnumerator PutCardIntoPlayFaceDown()
        {
            // Put the top card of your deck into play face down.
            if (TurnTaker.Deck.IsEmpty)
            {
                if (TurnTaker.Trash.IsEmpty)
                {
                    var e = GameController.SendMessageAction(TurnTaker.Name + "'s deck and trash are empty, so they can't put a card into play from their deck.", Priority.Medium, GetCardSource());
                    if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                    else { GameController.ExhaustCoroutine(e); }
                }
                else
                {
                    var e = GameController.ShuffleTrashIntoDeck(TurnTakerController, cardSource: GetCardSource());
                    if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                    else { GameController.ExhaustCoroutine(e); }
                }
            }

            var top = TurnTaker.Deck.TopCard;
            if (top == null) { yield break; }

            if (top.IsMissionCard)
            {
                // We know from Santa Guise that playing an OA Mission "face down" from your deck results in it entering play Reward side up.
                var e = GameController.SendMessageAction(CharacterCard.Title + "'s Charge produces a Reward!", Priority.Low, GetCardSource(), new Card[] { top }, true);
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }
            else
            {
                var e = GameController.FlipCard(FindCardController(top), cardSource: GetCardSource());
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }

            {
                var e = GameController.MoveCard(
                    TurnTakerController,
                    top,
                    TurnTaker.PlayArea,
                    playCardIfMovingToPlayArea: top.IsMissionCard,
                    responsibleTurnTaker: TurnTaker,
                    cardSource: GetCardSource()
                );
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }
        }

        private IEnumerator DischargeBattery()
        {
            // {Charge} {BatteryCharacter}...
            var e = this.ChargeCard(CharacterCard);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // ... until the start of your next turn.
            OnPhaseChangeStatusEffect chargeExpiration = new OnPhaseChangeStatusEffect(Card, nameof(ChargeExpiresResponse), "At the start of " + HeroTurnTakerController.Name + "'s next turn, {Discharge} " + Card.Title + ".", new TriggerType[] { TriggerType.ModifyStatusEffect }, Card);
            chargeExpiration.BeforeOrAfter = BeforeOrAfter.After;
            chargeExpiration.UntilStartOfNextTurn(TurnTaker);

            e = GameController.AddStatusEffect(chargeExpiration, true, GetCardSource());
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // Reveal each face-down card in your play area, then put them into play in any order.
            var faceDownCards = FindCardsWhere(
                c => c.IsFaceDownNonCharacter && !c.IsMissionCard && c.Location == TurnTaker.PlayArea,
                realCardsOnly: true,
                GetCardSource()
            );

            e = GameController.MoveCards(TurnTakerController, faceDownCards, TurnTaker.Revealed, responsibleTurnTaker: TurnTaker, cardSource: GetCardSource());
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
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator e;
            switch(index)
            {
                case 0: e = PutCardIntoPlayFaceDown(); break; 
                case 1: e = DischargeBattery(); break;
                default: yield break;
            }

            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }

        public IEnumerator ChargeExpiresResponse(PhaseChangeAction pca, OnPhaseChangeStatusEffect effect)
        {
            // It's the start of Battery's next turn, so remove Battery's Charge
            return this.DischargeCard(CharacterCard);
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator e;
            switch (index)
            {
                // One player may draw a card now.
                case 0:
                    e = GameController.SelectHeroToDrawCard(HeroTurnTakerController, cardSource: GetCardSource());
                    break;

                // Battery deals 1 target 2 melee damage.
                case 1:
                    e = GameController.SelectTargetsAndDealDamage(
                        HeroTurnTakerController,
                        new DamageSource(GameController, TurnTaker),
                        2,
                        DamageType.Lightning,
                        numberOfTargets: 1,
                        optional: false,
                        requiredTargets: 1,
                        cardSource: GetCardSource()
                    );
                    break;

                // Destroy a non-character card Equipment or Device card.
                case 2:
                    e = GameController.SelectAndDestroyCard(
                        HeroTurnTakerController,
                        new LinqCardCriteria(c => !c.IsCharacter && (c.Is(this).Equipment() || c.IsDevice), "non-character Equipment or Device"),
                        optional: false,
                        responsibleCard: CharacterCard,
                        cardSource: GetCardSource()
                    );
                    break;

                default: yield break;
            }

            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
