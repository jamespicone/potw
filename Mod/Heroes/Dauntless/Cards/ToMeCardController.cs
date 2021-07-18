using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dauntless
{
    public class ToMeCardController : CardController
    {
        public ToMeCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowListOfCardsAtLocation(HeroTurnTaker.Deck, new LinqCardCriteria((c) => c.DoKeywordsContain("relic"), "Relic"));
        }

        public override IEnumerator Play()
        {
            // "Search your deck for a Relic and put it into play. Shuffle your deck.",
            var e = SearchForCards(
                HeroTurnTakerController,
                searchDeck: true,
                searchTrash: false,
                minNumberOfCards: 1,
                maxNumberOfCards: 1,
                new LinqCardCriteria(c => c.DoKeywordsContain("relic")),
                putIntoPlay: true,
                putInHand: false,
                putOnDeck: false,
                shuffleAfterwards: true
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            // "Draw a card.
            e = DrawCard(HeroTurnTaker);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            // You may play a card"
            e = SelectAndPlayCardFromHand(HeroTurnTakerController);
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
