using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Leviathan
{
    public class LoseTheWarCardController : CardController
    {
        public LoseTheWarCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            // This card is indestructible
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            return card == Card;
        }

        public override void AddTriggers()
        {
            // At the start of the villain turn...
            AddStartOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => ExileEnvironment(pca),
                TriggerType.RemoveFromGame
            );
        }

        private IEnumerator GameOver(GameAction ga)
        {
            var e = GameController.GameOver(
                EndingResult.AlternateDefeat,
                "Leviathan has destroyed your surroundings!",
                actionSource: ga,
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

        private IEnumerator ExileEnvironment(PhaseChangeAction pca)
        {
            // Remove the top card of the environment deck from the game.
            IEnumerator e;
            var cardToMove = FindEnvironment().TurnTaker.Deck.TopCard;
            if (cardToMove == null) {
                // Failing that, remove the top card of the environment trash.
                e = GameController.SendMessageAction("The environment deck is empty...", Priority.Medium, GetCardSource(), showCardSource: true);
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }

                cardToMove = FindEnvironment().TurnTaker.Trash.TopCard;
                if (cardToMove == null)
                {
                    // Failing that, remove an environment card in play.
                    e = GameController.SendMessageAction("The environment trash is empty...", Priority.Medium, GetCardSource(), showCardSource: true);
                    if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                    else { GameController.ExhaustCoroutine(e); }

                    var storedCard = new List<SelectCardDecision>();
                    e = GameController.SelectCardAndStoreResults(
                        DecisionMaker,
                        SelectionType.RemoveCardFromGame,
                        new LinqCardCriteria(c => c.IsEnvironment && c.IsInPlay, "environment"),
                        storedResults: storedCard,
                        optional: false,
                        cardSource: GetCardSource()
                    );

                    if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                    else { GameController.ExhaustCoroutine(e); }

                    cardToMove = GetSelectedCard(storedCard);
                }
            }

            var moveResults = new List<MoveCardAction>();

            if (cardToMove != null)
            {
                e = GameController.MoveCard(
                    TurnTakerController,
                    cardToMove,
                    TurnTaker.OutOfGame,
                    showMessage: true,
                    responsibleTurnTaker: TurnTaker,
                    actionSource: pca,
                    storedResults: moveResults,
                    cardSource: GetCardSource()
                );

                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }

            // If no cards were removed the heroes lose the game.
            if (GetNumberOfCardsMoved(moveResults) <= 0)
            {
                e = GameController.GameOver(
                    EndingResult.AlternateDefeat,
                    "Leviathan has destroyed your surroundings!",
                    actionSource: pca,
                    cardSource: GetCardSource()
                );

                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }
        }
    }
}
