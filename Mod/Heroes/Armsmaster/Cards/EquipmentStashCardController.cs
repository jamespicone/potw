using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Armsmaster
{
    public class EquipmentStashCardController : CardController
    {
        public EquipmentStashCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowListOfCardsAtLocation(base.TurnTaker.Deck, new LinqCardCriteria((Card c) => c.DoKeywordsContain("equipment"), "equipment"));
        }

        public override IEnumerator Play()
        {
            // Search your deck for an Equipment card and either put it into play or into your hand. Shuffle your deck.
            var e = SearchForCards(
                HeroTurnTakerController,
                searchDeck: true,
                searchTrash: false,
                1,
                1,
                new LinqCardCriteria(c => c.DoKeywordsContain("equipment")),
                putIntoPlay: true,
                putInHand: true,
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
        }
    }
}
