using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dauntless
{
    public class OutfittingCardController : CardController
    {
        public OutfittingCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            foreach (var hero in Game.HeroTurnTakers)
            {
                SpecialStringMaker.ShowListOfCardsAtLocation(hero.Deck, new LinqCardCriteria((c) => c.DoKeywordsContain("equipment") || c.DoKeywordsContain("relic"), "Equipment or Relic"));
            }
        }

        public override IEnumerator Play()
        {
            // Select a hero.
            var storedResults = new List<SelectTurnTakerDecision>();
            var e = GameController.SelectHeroTurnTaker(
                HeroTurnTakerController,
                SelectionType.RevealCardsFromDeck,
                optional: false,
                allowAutoDecide: false,
                storedResults,
                associatedCards: new Card[] { Card },
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

            if (storedResults.Count() < 0) { yield break; }
            var selectedHero = storedResults.First().SelectedTurnTaker as HeroTurnTaker;
            if (selectedHero == null) { yield break; }
            var selectedHeroController = FindHeroTurnTakerController(selectedHero);
            if (selectedHeroController == null) { yield break; }

            // That hero reveals cards from the top of their deck until they reveal an Equipment or Relic card.
            var reveals = new List<RevealCardsAction>();
            e = GameController.RevealCards(
                selectedHeroController,
                selectedHero.Deck,
                c => c.DoKeywordsContain("equipment") || c.DoKeywordsContain("relic"),
                numberOfMatches: 1,
                reveals,
                RevealedCardDisplay.ShowRevealedCards,
                GetCardSource()
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            if (reveals.Count() <= 0) { yield break; }
            if (! reveals.First().FoundMatchingCards) { yield break; }
            if (reveals.First().MatchingCards.Count() <= 0) { yield break; }

            // They may put that card into play or into their hand.
            e = GameController.SelectLocationAndMoveCard(
                selectedHeroController,
                reveals.First().MatchingCards.First(),
                new MoveCardDestination[] {
                    new MoveCardDestination(selectedHero.PlayArea),
                    new MoveCardDestination(selectedHero.Hand)
                },
                isPutIntoPlay: true,
                responsibleTurnTaker: TurnTaker,
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

            // Shuffle the other revealed cards back into their deck"
            e = CleanupRevealedCards(selectedHero.Revealed, selectedHero.Deck, shuffleAfterwards: true);
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
