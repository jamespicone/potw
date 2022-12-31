using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;
using Handelabra;

namespace Jp.ParahumansOfTheWormverse.Skitter
{
    public class TrackingBugsCardController : CardController
    {
        public TrackingBugsCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => DoReveal(), TriggerType.RevealCard);

            this.AddResetTokenTrigger();
        }

        private IEnumerator DoReveal()
        {
            // "X on this card = 1 + the number of Bug tokens on this card.",
            // "At the end of your turn you may reveal the top X cards of a deck, discard up to 1 of those cards, then return the cards in any order."
            var deckList = new List<SelectLocationDecision>();
            var e = GameController.SelectADeck(
                HeroTurnTakerController,
                SelectionType.RevealCardsFromDeck,
                l => true,
                deckList,
                optional: false,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            var selectedDeck = GetSelectedLocation(deckList);
            if (selectedDeck == null) { yield break; }

            var revealCount = Card.BugTokenCount() + 1;
            var revealed = new List<Card>();
            var trash = FindTrashFromDeck(selectedDeck);

            e = GameController.RevealCards(
                HeroTurnTakerController,
                selectedDeck,
                revealCount,
                revealed,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            var allRevealed = revealed.ToList();

            var selectedDiscard = new List<SelectCardDecision>();
            e = GameController.SelectCardAndStoreResults(
                HeroTurnTakerController,
                SelectionType.DiscardCard,
                revealed,
                selectedDiscard,
                optional: true,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            var cardToDiscard = GetSelectedCard(selectedDiscard);
            if (cardToDiscard != null)
            {
                revealed.Remove(cardToDiscard);
                e = GameController.MoveCard(
                    TurnTakerController,
                    cardToDiscard,
                    trash,
                    isDiscard: true,
                    cardSource: GetCardSource()
                );
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }

            var selectedReturn = new List<SelectCardsDecision>();
            e = GameController.SelectCardsAndStoreResults(
                HeroTurnTakerController,
                SelectionType.ReturnToDeck,
                c => revealed.Contains(c),
                revealed.Count(),
                selectedReturn,
                optional: false,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            var returnCards = GetSelectedCards(selectedReturn).Reverse();
            foreach (Card c in returnCards)
            {
                e = GameController.MoveCard(
                    TurnTakerController,
                    c,
                    selectedDeck,
                    decisionSources: selectedReturn.CastEnumerable<SelectCardsDecision, IDecision>(),
                    cardSource: GetCardSource()
                );
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }

            e = CleanupCardsAtLocations(
                new List<Location> { selectedDeck.OwnerTurnTaker.Revealed },
                selectedDeck,
                cardsInList: allRevealed
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
