using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Coil
{
    public class TeleporterCardController : CardController
    {
        public TeleporterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"At the start of the villain turn...
            AddStartOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => FindTargets(),
                new TriggerType[] { TriggerType.RevealCard, TriggerType.PutIntoPlay }
            );
        }

        private IEnumerator FindTargets()
        {
            // reveal cards from the top of the villain deck until you have revealed H non-Device targets.
            // Put any revealed non-Device targets into play and shuffle the rest of the revealed cards back into the villain deck"
            var e = RevealCards_MoveMatching_ReturnNonMatchingCards(
                TurnTakerController,
                TurnTaker.Deck,
                playMatchingCards: false,
                putMatchingCardsIntoPlay: true,
                moveMatchingCardsToHand: false,
                new LinqCardCriteria(c => c.IsTarget && !c.IsDevice),
                numberOfMatches: H,
                revealedCardDisplay: RevealedCardDisplay.ShowMatchingCards,
                shuffleReturnedCards: true
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
