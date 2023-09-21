using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra;

using Jp.SOTMUtilities;
using Jp.ParahumansOfTheWormverse.Skitter;
using System.Reflection;

namespace Jp.ParahumansOfTheWormverse.Labyrinth
{

    public class LabyrinthCharacterCardController : HeroCharacterCardController
    {
        bool currentlyChangingTurnOrder = false;

        public LabyrinthCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            AddThisCardControllerToList(CardControllerListType.ChangesTurnTakerOrder);
        }

        public override int AskPriority => 5;

        private TurnTaker GetExpectedNextTurnTaker(TurnTaker active)
        {
            return GameController.FindNextAfterTurnTaker(active, cc => cc != this || cc.AskPriority <= AskPriority);
        }

        // {{Labyrinth}} takes her turn immediately after the first Environment turn, not in the usual hero turn order.
        public override TurnTaker AskIfTurnTakerOrderShouldBeChanged(TurnTaker fromTurnTaker, TurnTaker toTurnTaker)
        {
            // We've recursed; this can only happen if another cardcontroller is doing something similar.
            // We should return null just so everything bottoms out in an answer eventually.
            if (currentlyChangingTurnOrder)
                return null;

            var env = GameController.AllTurnTakers.First(tt => tt.Is().Environment());

            currentlyChangingTurnOrder = true;

            TurnTaker next = GetExpectedNextTurnTaker(fromTurnTaker);

            // If we're going to our turn and the previous turntaker wasn't the environment, skip our turn.
            if (next == TurnTaker && fromTurnTaker != env)
            {
                next = GetExpectedNextTurnTaker(next);
            }

            // If we're leaving the environment, we're the next turn
            if (fromTurnTaker == env)
            {
                next = TurnTaker;
            }

            // If we're leaving our turn, we should go to whoever is after the environment.
            if (fromTurnTaker == TurnTaker)
            {
                next = GetExpectedNextTurnTaker(env);
            }

            currentlyChangingTurnOrder = false;
            return next;
        }

        
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator e;
            switch(index)
            {
                // "Destroy an Environment card.",
                case 0:
                    e = GameController.SelectAndDestroyCard(
                        HeroTurnTakerController,
                        new LinqCardCriteria(c => c.Is().Environment(), "environment"),
                        optional: false,
                        responsibleCard: Card,
                        cardSource: GetCardSource()
                    );
                    break;

                // "One player plays a card.",
                case 1:
                    e = GameController.SelectHeroToPlayCard(HeroTurnTakerController, cardSource: GetCardSource());
                    break;

                // "One player draws a card."
                case 2:
                    e = GameController.SelectHeroToDrawCard(HeroTurnTakerController, cardSource: GetCardSource());
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
            // You may play a Shaping card. 
            var storedPlay = new List<PlayCardAction>();
            var e = SelectAndPlayCardFromHand(HeroTurnTakerController, storedResults: storedPlay, cardCriteria: new LinqCardCriteria(c => c.Is().WithKeyword("shaping").AccordingTo(this)));
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // If you don't, draw a card.
            if (! DidPlayCards(storedPlay))
            {
                e = DrawCard();
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }
        }

        public override IEnumerator BeforeFlipCardImmediateResponse(FlipCardAction flip)
        {
            if (CardWithoutReplacements.IsFlipped)
            {
                yield break;
            }

            var shapings = TurnTaker.GetAllCards().Where(c => c.IsInPlay && FindCardController(c) is ShapingCardController);
            var e = GameController.BulkMoveCards(TurnTakerController, shapings, TurnTaker.OutOfGame, cardSource: GetCardSource());
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            e = base.BeforeFlipCardImmediateResponse(flip);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
