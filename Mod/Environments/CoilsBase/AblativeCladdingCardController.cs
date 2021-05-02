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
    public class AblativeCladdingCardController : CoilsBaseSelfDestructCardController
    {
        public AblativeCladdingCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "Reduce all toxic and melee damage by 2."
            AddReduceDamageTrigger((DealDamageAction dda) => dda.DamageType == DamageType.Melee || dda.DamageType == DamageType.Toxic, (DealDamageAction dda) => 2);
        }
    }
}
