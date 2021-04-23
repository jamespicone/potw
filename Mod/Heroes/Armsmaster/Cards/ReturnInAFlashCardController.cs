using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Armsmaster
{
    public class ReturnInAFlashCardController : CardController
    {
        public ReturnInAFlashCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            LinqCardCriteria halberdCriteria = new LinqCardCriteria((Card c) => c.DoKeywordsContain("halberd"), "halberd");
            SpecialStringMaker.ShowListOfCardsAtLocation(base.TurnTaker.Deck, halberdCriteria);
        }

        public override IEnumerator Play()
        {
            // Reveal cards from the top of your deck or your trash until you reveal a Halberd. Either put it into play or into your hand. Return the other cards and shuffle your deck.
            var storedLocations = new List<SelectLocationDecision>();

            var e = GameController.SelectLocation(
                HeroTurnTakerController,
                new LocationChoice[2]
                {
                    new LocationChoice(TurnTaker.Deck),
                    new LocationChoice(TurnTaker.Trash)
                },
                SelectionType.RevealCardsFromDeck,
                storedLocations,
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

            Location revealingLocation = GetSelectedLocation(storedLocations);
            if (revealingLocation == null) { yield break; }

            e = RevealCards_MoveMatching_ReturnNonMatchingCards(
                TurnTakerController,
                revealingLocation,
                playMatchingCards: true,
                putMatchingCardsIntoPlay: false,
                moveMatchingCardsToHand: true,
                new LinqCardCriteria(c => c.DoKeywordsContain("halberd"), "Halberd"),
                numberOfMatches: 1,
                showMessage: true
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
