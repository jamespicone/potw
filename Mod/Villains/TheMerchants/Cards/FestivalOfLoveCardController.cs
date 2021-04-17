using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.TheMerchants
{
    public class FestivalOfLoveCardController : TheMerchantsUtilityCardController
    {
        public FestivalOfLoveCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "At the start of the villain turn, play the top card of the Thug deck."
            AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => base.GameController.PlayTopCardOfLocation(base.TurnTakerController, ThugDeck, responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource(), showMessage: true), TriggerType.PlayCard);
            base.AddTriggers();
        }
    }
}
