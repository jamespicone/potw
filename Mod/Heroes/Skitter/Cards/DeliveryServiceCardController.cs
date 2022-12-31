using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Skitter
{
    public class DeliveryServiceCardController : CardController
    {
        public DeliveryServiceCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            AddEndOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => GameController.SelectHeroToDrawCards(HeroTurnTakerController, Card.BugTokenCount() + 1, optionalDrawCards: false, requiredDraws: 0, allowAutoDraw: true, cardSource: GetCardSource()),
                TriggerType.DrawCard
            );

            this.AddResetTokenTrigger();
        }
    }
}
