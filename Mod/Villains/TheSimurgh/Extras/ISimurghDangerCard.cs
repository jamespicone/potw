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
    public interface ISimurghDangerCard
    {
        int Danger();
    }

    public static class SimurghUtility
    {
        public static int CompareSimurghDanger(Card lhs, Card rhs)
        {
            var l = lhs as ISimurghDangerCard;
            var r = rhs as ISimurghDangerCard;

            if (l == null && r == null) { return 0; }
            if (l == null) { return 1; }
            if (r == null) { return -1; }

            return l.Danger() - r.Danger();
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

            revealedCards.Sort(CompareSimurghDanger);
            revealedCards.Reverse();
            e = ttc.GameController.BulkMoveCards(ttc, revealedCards, ttc.TurnTaker.Deck, responsibleTurnTaker: ttc.TurnTaker, cardSource: source);
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