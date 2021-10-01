using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.BrocktonBay
{
    public class TheBoardwalkCardController : SuburbCardController
    {
        public TheBoardwalkCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "Reduce damage dealt to environment targets by 1."
            AddReduceDamageTrigger((Card c) => c.Alignment().Environment().Target(), 1);
            base.AddTriggers();
        }
    }
}
