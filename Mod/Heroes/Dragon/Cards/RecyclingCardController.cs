using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dragon
{
    public class RecyclingCardController : CardController
    {
        public RecyclingCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // Shuffle up to 2 equipment or device cards from your trash back into your deck
            var e = GameController.SelectCardsFromLocationAndMoveThem(
                HeroTurnTakerController,
                TurnTaker.Trash,
                minNumberOfCards: null,
                maxNumberOfCards: 2,
                new LinqCardCriteria(c => c.DoKeywordsContain("equipment") || c.DoKeywordsContain("device"), "equipment or device"),
                new MoveCardDestination[] { new MoveCardDestination(TurnTaker.Deck) },
                selectionType: SelectionType.MoveCardToHand,
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
        }
    }
}
