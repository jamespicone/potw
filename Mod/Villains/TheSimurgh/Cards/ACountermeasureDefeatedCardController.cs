using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.TheSimurgh
{
    public class ACountermeasureDefeatedCardController : CardController, ISimurghDangerCard
    {
        public ACountermeasureDefeatedCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public int Danger()
        {
            return 9;
        }

        public override IEnumerator Play()
        {
            // Reveal the top {H} cards of the villain trash.
            var revealedCards = new List<Card>();
            var e = GameController.RevealCards(
                TurnTakerController,
                TurnTaker.Trash,
                H,
                revealedCards,
                revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards,
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

            // Play the revealed card with the lowest {SimurghDanger}.
            var orderedCards = revealedCards.OrderBy(c => c.SimurghDanger(GameController)).Reverse();

            if (orderedCards.Count() > 0)
            {
                e = GameController.PlayCard(
                    TurnTakerController,
                    orderedCards.First(),
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

            e = CleanupRevealedCards(TurnTaker.Revealed, TurnTaker.Trash);
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
