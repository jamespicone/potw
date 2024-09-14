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
    public class FestivalOfLoveCardController : CardController
    {
        public FestivalOfLoveCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // At the start of the villain turn, play the top card of the Thug deck.
            AddStartOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => this.PlayThugs(),
                TriggerType.PlayCard
            );
        }
    }
}
