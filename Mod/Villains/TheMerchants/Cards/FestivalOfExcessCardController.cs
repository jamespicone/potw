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
    public class FestivalOfExcessCardController : TheMerchantsUtilityCardController
    {
        public FestivalOfExcessCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "Reduce damage dealt to villain targets by 1."
            AddReduceDamageTrigger((Card c) => c.IsVillainTarget, 1);
            base.AddTriggers();
        }
    }
}
