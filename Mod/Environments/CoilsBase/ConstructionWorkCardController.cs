using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.CoilsBase
{
    public class ConstructionWorkCardController : CardController
    {
        public ConstructionWorkCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // "At the end of the environment turn, reveal cards from the top of the environment deck until you reveal three Structure cards. Put them into play and shuffle the other revealed cards back into the environment deck. Then, destroy this card."
            AddEndOfTurnTrigger(
                tt => tt == TurnTaker,
                SearchDestructResponse,
                new TriggerType[] { TriggerType.RevealCard, TriggerType.PutIntoPlay, TriggerType.ShuffleDeck, TriggerType.DestroySelf }
            );
        }

        public IEnumerator SearchDestructResponse(GameAction ga)
        {
            // "... reveal cards from the top of the environment deck until you reveal three Structure cards. Put them into play and shuffle the other revealed cards back into the environment deck."
            var e = RevealCards_MoveMatching_ReturnNonMatchingCards(
                TurnTakerController,
                TurnTaker.Deck,
                playMatchingCards: false,
                putMatchingCardsIntoPlay: true,
                moveMatchingCardsToHand: false,
                new LinqCardCriteria(c => c.DoKeywordsContain("structure"), "structure"),
                numberOfMatches: 3,
                shuffleSourceAfterwards: true,
                revealedCardDisplay: RevealedCardDisplay.ShowMatchingCards
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            // "Then, destroy this card."
            e = GameController.DestroyCard(
                DecisionMaker,
                Card,
                responsibleCard: Card,
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
