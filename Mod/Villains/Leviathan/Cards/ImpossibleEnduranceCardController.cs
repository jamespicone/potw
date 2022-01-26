using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Leviathan
{
    public class ImpossibleEnduranceCardController : CardController
    {
        public ImpossibleEnduranceCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // At the start of the villain turn, shuffle the villain trash then reveal cards from the top of the villain trash until you reveal a one-shot.
            // Play it, and then shuffle the other revealed cards back into the villain trash
            AddStartOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => FindOneShot(pca),
                TriggerType.PlayCard
            );
        }

        private IEnumerator FindOneShot(PhaseChangeAction pca)
        {
            var e = ShuffleDeck(DecisionMaker, TurnTaker.Trash);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            e = RevealCards_MoveMatching_ReturnNonMatchingCards(
                TurnTakerController,
                TurnTaker.Trash,
                playMatchingCards: true,
                putMatchingCardsIntoPlay: false,
                moveMatchingCardsToHand: false,
                new LinqCardCriteria(c => c.DoKeywordsContain("one-shot"), "one-shot"),
                numberOfMatches: 1,
                revealedCardDisplay: RevealedCardDisplay.ShowMatchingCards
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
