
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.JessicaYamada
{
    class PsychologicalTrainingCardController : CardController
    {
        public PsychologicalTrainingCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // At the end of your turn, reveal the top card of your deck. If it's a One-Shot, put it into your hand. Otherwise, return it to the top of the deck
            AddEndOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => RevealAndDraw(),
                TriggerType.DrawCard
            );
        }

        public IEnumerator RevealAndDraw()
        {
            var e = RevealCards_MoveMatching_ReturnNonMatchingCards(
                TurnTakerController,
                TurnTaker.Deck,
                playMatchingCards: false,
                putMatchingCardsIntoPlay: false,
                moveMatchingCardsToHand: true,
                new LinqCardCriteria(c => c.IsOneShot, "One-Shot"),
                numberOfMatches: null,
                numberOfCards: 1,
                shuffleSourceAfterwards: false,
                revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards
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
