using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Alexandria
{

    /*
     * Variant thoughts:
     * - This becomes Alexandria, Cauldron Cape (because it's about exploiting her thinker power)
     * - Regular Alexandria gives other targets Wraith-like reduce-next-damage + draws a card (because she wants to protect people and is learning her abilities)
     * - Pretender reveals 3 from the top of their deck and may play one (because he doesn't have a great handle on how to do the things she does)
     * 
     * Might need more return-to-hand sources than Alexandria's Cape. Maybe some of the one-shots let you do it (time-locked and pure strength maybe?)
     * 
     * Incap thoughts:
     * - base character should probably emphasise protecting other heroes, helping them learn, and maybe "she's got a bit of a dark side" somehow
     * - pretender version should revive dead targets
     */

    public class AlexandriaCharacterCardController : HeroCharacterCardController
    {
        public AlexandriaCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator e;

            switch (index)
            {
                case 0:
                    //"Select a target. Until the start of your next turn reduce damage dealt to that target by 1",
                    e = ProtectSomeone();
                    break;

                case 1:
                    //"One player may search their deck for a card and put it into their hand",
                    e = SomeoneSearches();
                    break;

                case 2:
                    //"{AlexandriaCharacter} deals 2 melee damage to a target"
                    e = GameController.SelectTargetsAndDealDamage(
                        HeroTurnTakerController,
                        new DamageSource(GameController, TurnTaker),
                        amount: 2,
                        DamageType.Melee,
                        numberOfTargets: 1,
                        optional: false,
                        requiredTargets: 1,
                        cardSource: GetCardSource()
                    );
                    break;

                default:
                    yield break;                    
            }

            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // You may play a card.
            var e = SelectAndPlayCardFromHand(HeroTurnTakerController);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            // Return one of your cards in play to your hand
            e = GameController.SelectAndMoveCard(
                HeroTurnTakerController,
                c => !c.IsCharacter && c.IsInPlay && c.Location == TurnTaker.PlayArea && c.Owner == TurnTaker,
                HeroTurnTaker.Hand,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        private IEnumerator ProtectSomeone()
        {
            // "Select a target. Until the start of your next turn reduce damage dealt to that target by 1",
            var selected = new List<SelectCardDecision>();
            var e = GameController.SelectCardAndStoreResults(
                HeroTurnTakerController,
                SelectionType.ReduceDamageTaken,
                new LinqCardCriteria(c => c.IsTarget && c.IsInPlay, "target"),
                selected,
                optional: false,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var target = GetSelectedCard(selected);
            if (target == null) { yield break; }

            var effect = new ReduceDamageStatusEffect(1);

            effect.TargetCriteria.IsSpecificCard = target;
            effect.UntilStartOfNextTurn(TurnTaker);
            effect.UntilTargetLeavesPlay(target);

            e = AddStatusEffect(effect);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        private IEnumerator SomeoneSearches()
        {
            var storedResults = new List<SelectTurnTakerDecision>();
            var e = GameController.SelectHeroTurnTaker(
                HeroTurnTakerController,
                SelectionType.SearchDeck,
                optional: false,
                allowAutoDecide: false,
                storedResults,
                numberOfCards: 1,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var selected = GetSelectedTurnTaker(storedResults) as HeroTurnTaker;
            if (selected == null) { yield break; }

            var selectedController = FindHeroTurnTakerController(selected);
            if (selectedController == null) { yield break; }

            e = GameController.SelectCardFromLocationAndMoveIt(
                selectedController,
                selected.Deck,
                new LinqCardCriteria(),
                new MoveCardDestination[] {
                    new MoveCardDestination(selected.Hand)
                },
                shuffleAfterwards: true,
                responsibleTurnTaker: TurnTaker,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }
    }
}
