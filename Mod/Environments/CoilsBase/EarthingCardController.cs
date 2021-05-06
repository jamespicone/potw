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
    public class EarthingCardController : CoilsBaseSelfDestructCardController
    {
        public EarthingCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "Reduce all cold and lightning damage by 2."
            AddReduceDamageTrigger((DealDamageAction dda) => dda.DamageType == DamageType.Cold || dda.DamageType == DamageType.Lightning, (DealDamageAction dda) => 2);
        }
    }
}
