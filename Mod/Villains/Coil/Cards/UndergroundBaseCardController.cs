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
    public class UndergroundBaseCardController : CardController
    {
        public UndergroundBaseCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // Villain targets are immune to damage from Environment cards
            AddImmuneToDamageTrigger(dda => dda.DamageSource.Is().Environment() && dda.Target.Is(this).Villain().Target());
        }
    }
}
