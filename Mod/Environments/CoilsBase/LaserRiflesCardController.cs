using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            base.AddTriggers();
            // "Increase damage dealt by villain targets by 1."
            AddIncreaseDamageTrigger((DealDamageAction dda) => dda.DamageSource != null && dda.DamageSource.IsVillainTarget, (DealDamageAction dda) => 1);
            // "Change the type of that damage to energy."
            AddChangeDamageTypeTrigger((DealDamageAction dda) => dda.DamageSource != null && dda.DamageSource.IsVillainTarget, DamageType.Energy);
        }
    }
}
