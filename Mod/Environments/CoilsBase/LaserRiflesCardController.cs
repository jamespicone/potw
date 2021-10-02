using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.CoilsBase
{
    public class LaserRiflesCardController : CardController
    {
        public LaserRiflesCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // "Increase damage dealt by villain targets by 1."
            AddIncreaseDamageTrigger(dda => dda.DamageSource != null && dda.DamageSource.Is(this).Villain().Target(), dda => 1);

            // "Change the type of that damage to energy."
            AddChangeDamageTypeTrigger(dda => dda.DamageSource != null && dda.DamageSource.Is(this).Villain().Target(), DamageType.Energy);
        }
    }
}
