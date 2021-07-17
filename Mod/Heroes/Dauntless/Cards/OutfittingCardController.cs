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
        { }

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

            // That hero reveals cards from the top of their deck until they reveal an Equipment card.
            // They may put that card into play or into their hand.
            // Shuffle the other revealed cards back into their deck"
            e = RevealCards_SelectSome_MoveThem_ReturnTheRest(
                selectedHeroController,
                selectedHeroController,
                selectedHero.Deck,
                c => c.DoKeywordsContain("equipment"),
                numberOfMatchesToReveal: 1,
                numberOfRevealedCardsToChoose: 1,
                canPutInHand: true,
                canPlayCard: true,
                isPutIntoPlay: true,
                "Equipment"
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
