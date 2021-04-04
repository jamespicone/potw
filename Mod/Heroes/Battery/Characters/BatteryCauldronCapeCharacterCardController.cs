using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                    if (base.TurnTaker.Deck.IsEmpty)
                    {
                        if (base.TurnTaker.Trash.IsEmpty)
                        {
                            IEnumerator messageCoroutine = base.GameController.SendMessageAction(base.TurnTaker.Name + "'s deck and trash are empty, so they can't put a card into play from their deck.", Priority.Medium, GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(messageCoroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(messageCoroutine);
                            }
                        }
                        else
                        {
                            IEnumerator shuffleCoroutine = base.GameController.ShuffleTrashIntoDeck(base.TurnTakerController, cardSource: GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(shuffleCoroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(shuffleCoroutine);
                            }
                        }
                    }
                    Card top = base.TurnTaker.Deck.TopCard;
                    if (top == null)
                    {
                        break;
                    }
                    else if (top.IsMissionCard)
                    {
                        // We know from Santa Guise that playing an OA Mission "face down" from your deck results in it entering play Reward side up.
                        IEnumerator missionCoroutine = base.GameController.SendMessageAction(base.CharacterCard.Title + "'s Charge produces a Reward!", Priority.Low, GetCardSource(), new Card[] { top }, true);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(missionCoroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(missionCoroutine);
                        }
                    }
                    else
                    {
                        IEnumerator prepareCoroutine = base.GameController.FlipCard(FindCardController(top), cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(prepareCoroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(prepareCoroutine);
                        }
                    }
                    IEnumerator moveCoroutine = base.GameController.MoveCard(base.TurnTakerController, top, base.TurnTaker.PlayArea, playCardIfMovingToPlayArea: top.IsMissionCard, responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(moveCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(moveCoroutine);
                    }
                    break;
                case 1:
                    // "{Charge} {BatteryCharacter}..."
                    IEnumerator chargeCoroutine = base.Charge(base.Card);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(chargeCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(chargeCoroutine);
                    }
                    // "... until the start of your next turn."
                    OnPhaseChangeStatusEffect chargeExpiration = new OnPhaseChangeStatusEffect(base.Card, nameof(ChargeExpiresResponse), "At the start of " + base.HeroTurnTakerController.Name + "'s next turn, {Discharge} " + base.Card.Title + ".", new TriggerType[] { TriggerType.MoveCard }, base.Card);
                    chargeExpiration.NumberOfUses = 1;
                    chargeExpiration.BeforeOrAfter = BeforeOrAfter.After;
                    chargeExpiration.TurnTakerCriteria.IsSpecificTurnTaker = base.TurnTaker;
                    chargeExpiration.TurnPhaseCriteria.Phase = Phase.Start;
                    chargeExpiration.TurnPhaseCriteria.TurnTaker = base.TurnTaker;
                    chargeExpiration.TurnIndexCriteria.GreaterThan = base.Game.TurnIndex;
                    IEnumerator expireCoroutine = base.GameController.AddStatusEffect(chargeExpiration, true, GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(expireCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(expireCoroutine);
                    }
                    // "Flip each face-down card in your play area, treating them as just put into play."
                    IEnumerator flipCoroutine = base.GameController.SelectAndFlipCards(base.HeroTurnTakerController, new LinqCardCriteria((Card c) => c.Location == base.TurnTaker.PlayArea && c.IsFaceDownNonCharacter && !c.IsMissionCard, "face-down cards in " + base.TurnTaker.Name + "'s play area", false, false, "face-down card in " + base.TurnTaker.Name + "'s play area", "face-down cards in " + base.TurnTaker.Name + "'s play area"), null, toFaceDown: false, optional: false, treatAsPutIntoPlay: true, choiceOrdering: (Card c) => c.PlayIndex.Value, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(flipCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(flipCoroutine);
                    }
                    break;
            }
            yield break;
        }

        public IEnumerator ChargeExpiresResponse(PhaseChangeAction pca, OnPhaseChangeStatusEffect effect)
        {
            // It's the start of Battery's next turn, so remove Battery's Charge
            //Log.Debug("ChargeExpiresResponse activated");
            IEnumerator removeCoroutine = base.RemoveCharge(base.Card);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(removeCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(removeCoroutine);
            }
            yield break;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator incapCoroutine;
            switch (index)
            {
                case 0:
                    incapCoroutine = UseIncapOption1();
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(incapCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(incapCoroutine);
                    }
                    break;
                case 1:
                    incapCoroutine = UseIncapOption2();
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(incapCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(incapCoroutine);
                    }
                    break;
                case 2:
                    incapCoroutine = UseIncapOption3();
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(incapCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(incapCoroutine);
                    }
                    break;
            }
            yield break;
        }

        private IEnumerator UseIncapOption1()
        {
            // "One player may draw a card now."
            IEnumerator drawCoroutine = base.GameController.SelectHeroToDrawCard(base.HeroTurnTakerController, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(drawCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(drawCoroutine);
            }
            yield break;
        }

        private IEnumerator UseIncapOption2()
        {
            // "One hero target deals 1 target 2 melee damage."
            List<SelectCardDecision> chooseHeroTarget = new List<SelectCardDecision>();
            IEnumerator chooseHeroCoroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.CardToDealDamage, new LinqCardCriteria((Card c) => c.IsInPlay && c.IsTarget && c.IsHero, "hero target", useCardsSuffix: false), chooseHeroTarget, false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(chooseHeroCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(chooseHeroCoroutine);
            }
            SelectCardDecision choice = chooseHeroTarget.FirstOrDefault();
            if (choice != null && choice.SelectedCard != null)
            {
                IEnumerator damageCoroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, choice.SelectedCard), 2, DamageType.Melee, 1, false, 1, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(damageCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(damageCoroutine);
                }
            }
            yield break;
        }

        private IEnumerator UseIncapOption3()
        {
            // "Destroy a non-character card Equipment or Device card."
            IEnumerator destroyCoroutine = base.GameController.SelectAndDestroyCard(base.HeroTurnTakerController, new LinqCardCriteria((Card c) => !c.IsCharacter && (c.DoKeywordsContain("equipment") || c.IsDevice), "non-character Equipment or Device"), false, responsibleCard: base.Card, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(destroyCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(destroyCoroutine);
            }
            yield break;
        }
    }
}
