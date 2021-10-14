using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Grue
{
    public class StillnessCardController : CardController
    {
        public StillnessCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // Reduce all fire, sonic, lightning, and energy damage that would be dealt to or by a target with a Darkness card next to it by 1
            //
            // This is one trigger that fires once even if target and source both have darkness adjacent deliberately; that's my intended interpretation
            // of the card.
            AddReduceDamageTrigger(
                dda => 
                    (this.DoesTargetHaveDarknessAdjacent(dda.Target) || this.DoesTargetHaveDarknessAdjacent(dda.DamageSource.Card)) &&
                    (
                        dda.DamageType == DamageType.Fire ||
                        dda.DamageType == DamageType.Sonic ||
                        dda.DamageType == DamageType.Lightning ||
                        dda.DamageType == DamageType.Energy
                    ),
                dda => 1);
        }
    }
}
