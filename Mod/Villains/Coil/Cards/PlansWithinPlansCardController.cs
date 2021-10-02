using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Coil
{
    public class PlansWithinPlansCardController : CardController
    {
        public PlansWithinPlansCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // "Reduce damage dealt to villain targets by 2"
            AddReduceDamageTrigger(c => c.Is(this).Villain().Target(), 2);
        }
    }
}
