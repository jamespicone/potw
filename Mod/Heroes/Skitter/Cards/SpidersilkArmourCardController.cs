using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Skitter
{
    public class SpidersilkArmourCardController : CardController
    {
        public SpidersilkArmourCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // "Reduce non-fire damage that would be dealt to {SkitterCharacter} by 1.
            AddReduceDamageTrigger(dda => dda.Target == CharacterCard && dda.DamageType != DamageType.Fire, dda => 1);
        }
    }
}
