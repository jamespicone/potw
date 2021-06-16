using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dragon
{
    public class BorrowedBlueprintsCardController : CardController
    {
        public BorrowedBlueprintsCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            /* Select a player other than Dragon.
             * Reveal the top X cards of your deck, where X = the number of Equipment and Device cards that player has in hand + 2.
             * Put any revealed Equipment or Device cards into your hand, shuffle the rest back into your deck"
             */
            var storedTurnTaker = new List<SelectTurnTakerDecision>();
            var e = GameController.SelectHeroTurnTaker(
                HeroTurnTakerController,
                SelectionType.AmbiguousDecision,
                optional: false,
                allowAutoDecide: false,
                storedTurnTaker,
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

            var selectedTurnTaker = GetSelectedTurnTaker(storedTurnTaker) as HeroTurnTaker;
            if (selectedTurnTaker == null) { yield break; }

            var cardsToReveal = selectedTurnTaker.Hand.Cards.Count(c => c.DoKeywordsContain("equipment") || c.DoKeywordsContain("device")) + 2;

            e = RevealCards_MoveMatching_ReturnNonMatchingCards(
                TurnTakerController,
                TurnTaker.Deck,
                playMatchingCards: false,
                putMatchingCardsIntoPlay: false,
                moveMatchingCardsToHand: true,
                new LinqCardCriteria(c => c.DoKeywordsContain("equipment") || c.DoKeywordsContain("device"), "equipment or device"),
                numberOfMatches: null,
                numberOfCards: cardsToReveal,
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
