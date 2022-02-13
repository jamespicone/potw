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
    abstract public class SimurghPlayWhenRevealedCardController : CardController
    {
        public SimurghPlayWhenRevealedCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        protected abstract string SurpriseMessage();

        public override void AddStartOfGameTriggers()
        {
            // "When this card is revealed, play it.",
            AddTrigger<RevealCardsAction>(
                rca => rca.RevealedCards.Contains(Card),
                rca => Surprise(rca),
                TriggerType.PlayCard,
                TriggerTiming.After,
                outOfPlayTrigger: true
            );
        }

        private IEnumerator Surprise(RevealCardsAction rca)
        {
            rca.RemoveCardFromRevealedCards(Card);

            var e = GameController.SendMessageAction(
                SurpriseMessage(),
                Priority.High,
                GetCardSource(),
                showCardSource: true
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            // The precedent set by Shinobi Assassin is that you reveal a new card to make up for this one.
            // Personally I disagree and think it's a particularly gross example of Sentinel's unwritten rules,
            // but the precedent is clear so I'm going to implement it.
            if (rca.RevealCardsUntil == null)
            {
                var storedResults = new List<Card>();
                e = GameController.RevealCards(
                    TurnTakerController,
                    rca.SearchLocation,
                    1,
                    storedResults, rca.FromBottom,
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

                foreach (Card c in storedResults)
                {
                    rca.AddCardToRevealedCards(c);
                }
            }

            e = GameController.PlayCard(
                TurnTakerController,
                Card,
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
