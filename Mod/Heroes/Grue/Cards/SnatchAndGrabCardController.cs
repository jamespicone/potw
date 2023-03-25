using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Grue
{
    public class SnatchAndGrabCardController : CardController
    {
        public SnatchAndGrabCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // "Destroy up to 2 Ongoing cards"
            return GameController.SelectAndDestroyCards(
                HeroTurnTakerController,
                new LinqCardCriteria(c => IsOngoing(c), "ongoing"),
                numberOfCards: 2,
                requiredDecisions: 0,
                responsibleCard: Card,
                cardSource: GetCardSource()
            );
        }
    }
}
