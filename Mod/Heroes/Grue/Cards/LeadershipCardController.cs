﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Grue
{
    public class LeadershipCardController : CardController
    {
        public LeadershipCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // "Increase damage dealt by hero targets by 1"
            AddIncreaseDamageTrigger(dda => dda.DamageSource.Is().Hero().Target(), 1);
        }
    }
}