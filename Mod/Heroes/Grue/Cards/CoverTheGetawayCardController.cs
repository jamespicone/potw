using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Grue
{
    public class CoverTheGetawayCardController : CardController
    {
        public CoverTheGetawayCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // "Put a Darkness card next to each hero character card",
            var e = GameController.SelectCardsAndDoAction(
                HeroTurnTakerController,
                new LinqCardCriteria(c => c.Is().Hero().Character(), "hero character"),
                SelectionType.MoveCardNextToCard,
                c => this.PutDarknessIntoPlay(c),
                allowAutoDecide: true,
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

            // "You may destroy an Environment card"
            e = GameController.SelectAndDestroyCard(
                HeroTurnTakerController,
                new LinqCardCriteria(c => c.Is().Environment(), "environment"),
                optional: true,
                responsibleCard: Card,
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
