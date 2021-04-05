using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Tattletale
{
    public class ReadingCardController : CardController
    {
        public ReadingCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddTriggers();
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // "Reveal the top 3 cards of a deck. Put 1 on the top and 2 on the bottom."
            int numRevealed = GetPowerNumeral(0, 3);
            int numToTop = GetPowerNumeral(1, 1);
            int numToBottom = GetPowerNumeral(2, 2);
            // Choose a deck
            List<SelectLocationDecision> chosen = new List<SelectLocationDecision>();
            IEnumerator chooseDeckCoroutine = base.GameController.SelectADeck(base.HeroTurnTakerController, SelectionType.RevealCardsFromDeck, (Location l) => l.HasCards, chosen, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(chooseDeckCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(chooseDeckCoroutine);
            }
            Location deck = GetSelectedLocation(chosen);
            if (deck != null)
            {
                // Reveal cards
                RevealedCardDisplay display = RevealedCardDisplay.None;
                if (deck.Cards.Count() < 3)
                {
                    display = RevealedCardDisplay.ShowRevealedCards;
                }
                List<Card> revealedCards = new List<Card>();
                IEnumerator revealCoroutine = base.GameController.RevealCards(base.TurnTakerController, deck, numRevealed, revealedCards, false, display, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(revealCoroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(revealCoroutine);
                }
                if (revealedCards.Count() > numToTop)
                {
                    // Select cards to move to top
                    IEnumerator topCoroutine = base.GameController.SelectCardsFromLocationAndMoveThem(base.HeroTurnTakerController, deck.OwnerTurnTaker.Revealed, numToTop, numToTop, new LinqCardCriteria((Card c) => revealedCards.Contains(c)), new MoveCardDestination[] { new MoveCardDestination(deck) }, responsibleTurnTaker: base.TurnTaker, selectionType: SelectionType.MoveCardOnDeck, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(topCoroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(topCoroutine);
                    }
                    if (revealedCards.Count() > 0)
                    {
                        // Move to bottom
                        IEnumerator bottomCoroutine = base.GameController.SelectCardsFromLocationAndMoveThem(base.HeroTurnTakerController, deck.OwnerTurnTaker.Revealed, numToBottom, numToBottom, new LinqCardCriteria((Card c) => revealedCards.Contains(c)), new MoveCardDestination[] { new MoveCardDestination(deck, toBottom: true) }, responsibleTurnTaker: base.TurnTaker, selectionType: SelectionType.MoveCardOnBottomOfDeck, cardSource: GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(bottomCoroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(bottomCoroutine);
                        }
                    }
                }
                else
                {
                    // Move ALL of them to top
                    IEnumerator topCoroutine = base.GameController.MoveCards(base.TurnTakerController, revealedCards, deck, responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(topCoroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(topCoroutine);
                    }
                }
                // Clean up afterward
                List<Card> remaining = revealedCards.Where((Card c) => c.Location.IsRevealed).ToList();
                if (remaining.Count() > 0)
                {
                    List<Location> toClean = new List<Location>();
                    toClean.Add(deck.OwnerTurnTaker.Revealed);
                    IEnumerator cleanCoroutine = CleanupCardsAtLocations(toClean, deck, cardsInList: remaining);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(cleanCoroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(cleanCoroutine);
                    }
                }
            }
            yield break;
        }
    }
}
