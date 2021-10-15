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
    public class TheDocksCardController : SuburbCardController
    {
        public TheDocksCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "Increase damage dealt by environment targets by 1."
            AddIncreaseDamageTrigger((DealDamageAction dda) => dda.DamageSource.Is().Environment().Target(), (DealDamageAction dda) => 1);
            base.AddTriggers();
        }
    }
}
