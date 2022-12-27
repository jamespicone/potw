using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Bitch
{
    public class HeelCardController : CardController
    {
        public HeelCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Deck, new LinqCardCriteria((Card c) => c.DoKeywordsContain("dog"), "dog"));
            SpecialStringMaker.ShowLocationOfCards(new LinqCardCriteria((Card c) => c.Identifier == "Bastard" && c.Owner == TurnTaker));
        }

        public override IEnumerator Play()
        {
            // "Reveal cards from the top of either your deck or your trash until you reveal a Dog. Put it into play. Return the other cards and shuffle your deck.",
            // "You may draw a card"
            var storedResults = new List<SelectLocationDecision>();
            var e = GameController.SelectLocation(
                HeroTurnTakerController,
                new LocationChoice[] {
                    new LocationChoice(HeroTurnTaker.Deck),
                    new LocationChoice(HeroTurnTaker.Trash)
                },
                SelectionType.RevealCardsFromDeck,
                storedResults,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            var selectedLocation = GetSelectedLocation(storedResults);
            if (selectedLocation == null) { yield break; }

            e = RevealCards_MoveMatching_ReturnNonMatchingCards(
                TurnTakerController,
                selectedLocation,
                false,
                true,
                false,
                new LinqCardCriteria(c => c.DoKeywordsContain("dog")),
                1,
                null,
                revealedCardDisplay: RevealedCardDisplay.ShowMatchingCards,
                shuffleReturnedCards: true
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            e = DrawCard(HeroTurnTaker, optional: true);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
