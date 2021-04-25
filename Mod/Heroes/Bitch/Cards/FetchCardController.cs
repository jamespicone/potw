using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Bitch
{
    public class FetchCardController : CardController
    {
        public FetchCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("dog"), "dog"));
            SpecialStringMaker.ShowNumberOfCardsAtLocations(() => from ttc in base.GameController.FindTurnTakerControllersWhere((TurnTakerController ttc) => true, cardSource: GetCardSource()) select ttc.TurnTaker.Deck, new LinqCardCriteria((Card c) => (c.DoKeywordsContain("equipment") || c.DoKeywordsContain("device")), "equipment or device"));
        }

        public override System.Collections.IEnumerator Play()
        {
            var cards = GameController.FindCardsWhere(card => card.IsInPlay && card.DoKeywordsContain("dog"), true, GetCardSource());
            var dogCount = cards.Count();

            // Reveal the top X cards of a deck, where X = the number of Dog cards in play + 2. For each equipment or device card revealed in this way, either discard it or put it into play. Shuffle the other cards back into the deck
            var storedResults = new List<SelectLocationDecision>();
            var e = SelectDecks(HeroTurnTakerController, 1, SelectionType.RevealCardsFromDeck, l => true, storedResults);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var deck = storedResults.FirstOrDefault();
            if (deck == null) { yield break; }

            var storedReveal = new List<Card>();
            e = GameController.RevealCards(
                TurnTakerController,
                deck.SelectedLocation.Location,
                2 + dogCount,
                storedReveal,
                revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards,
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

            foreach (Card c in storedReveal)
            {
                if (!c.DoKeywordsContain(new string[]{ "equipment", "device" })) { continue; }

                var theCard = new List<Card>();
                theCard.Add(c);

                var decisions = new List<Function>();
                decisions.Add(new Function(HeroTurnTakerController, "Discard", SelectionType.DiscardFromDeck, () => GameController.MoveCard(TurnTakerController, c, c.NativeTrash)));
                decisions.Add(new Function(HeroTurnTakerController, "Put into play", SelectionType.PutIntoPlay, () => GameController.PlayCard(TurnTakerController, c, isPutIntoPlay: true)));

                e = GameController.SelectAndPerformFunction(
                    new SelectFunctionDecision(GameController, HeroTurnTakerController, decisions, false, associatedCards: theCard, cardSource: GetCardSource())
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

            e = CleanupRevealedCards(deck.SelectedLocation.Location.OwnerTurnTaker.Revealed, deck.SelectedLocation.Location, shuffleAfterwards: true);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            // TODO: Informational dog count display
        }
    }
}
