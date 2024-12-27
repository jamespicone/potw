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
                pca => GameController.SelectHeroToDrawCards(
                    HeroTurnTakerController,
                    Math.Min(3, Card.BugTokenCount()) + 1,
                    optionalDrawCards: false,
                    requiredDraws: -1, // There's an off-by-one error in SelectHeroToDrawCards
                    allowAutoDraw: true,
                    cardSource: GetCardSource()
                ),
                TriggerType.DrawCard
            );

            this.AddResetTokenTrigger();
        }
    }
}
