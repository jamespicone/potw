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
    public class BlastDoorsCardController : CoilsBaseSelfDestructCardController
    {
        public BlastDoorsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "Reduce all fire and projectile damage by 2."
            AddReduceDamageTrigger((DealDamageAction dda) => dda.DamageType == DamageType.Fire || dda.DamageType == DamageType.Projectile, (DealDamageAction dda) => 2);
        }
    }
}
