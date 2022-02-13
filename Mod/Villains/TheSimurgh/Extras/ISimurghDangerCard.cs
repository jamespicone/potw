using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.TheSimurgh
{
    public interface ISimurghDangerCard
    {
        int Danger();
    }

    public static class SimurghUtility
    {
        public static int SimurghDanger(this Card c, GameController g)
        {
            var controller = g.FindCardController(c) as ISimurghDangerCard;
            if (controller == null) { return int.MaxValue; }

            return controller.Danger();
        }

        public static IEnumerator StackDeck(this TurnTakerController ttc, int numberOfCards, CardSource source)
        {
            var revealedCards = new List<Card>();
            var e = ttc.GameController.RevealCards(
                ttc,
                ttc.TurnTaker.Deck,
                numberOfCards,
                revealedCards,
                revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards,
                cardSource: source
            );
            if (ttc.UseUnityCoroutines)
            {
                yield return ttc.GameController.StartCoroutine(e);
            }
            else
            {
                ttc.GameController.ExhaustCoroutine(e);
            }

            e = ttc.GameController.BulkMoveCards(ttc, revealedCards.OrderBy(c => c.SimurghDanger(ttc.GameController)).Reverse(), ttc.TurnTaker.Deck, responsibleTurnTaker: ttc.TurnTaker, cardSource: source);
            if (ttc.UseUnityCoroutines)
            {
                yield return ttc.GameController.StartCoroutine(e);
            }
            else
            {
                ttc.GameController.ExhaustCoroutine(e);
            }
        }
    }
}