using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dauntless
{
    public class ResolveCardController : CardController
    {
        public ResolveCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowListOfCardsAtLocation(HeroTurnTaker.Deck, new LinqCardCriteria((c) => c.DoKeywordsContain("charge"), "Charge"));
            SpecialStringMaker.ShowListOfCardsAtLocation(HeroTurnTaker.Trash, new LinqCardCriteria((c) => c.DoKeywordsContain("charge"), "Charge"));
        }

        public override IEnumerator Play()
        {
            // "Reveal cards from the top of your trash or the top of your deck until you reveal a Charge card.
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
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var location = GetSelectedLocation(storedResults);
            if (location == null) { yield break; }

            // You may put it into play or into your hand.
            // Shuffle the other cards back where they came from"
            e = RevealCards_SelectSome_MoveThem_ReturnTheRest(
                HeroTurnTakerController,
                HeroTurnTakerController,
                location,
                c => c.DoKeywordsContain("charge"),
                numberOfMatchesToReveal: 1,
                numberOfRevealedCardsToChoose: 1,
                canPutInHand: true,
                canPlayCard: true,
                isPutIntoPlay: true,
                "Charge"
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
