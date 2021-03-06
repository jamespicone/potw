using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Bitch
{
    public class HaveARideCardController : CardController
    {
        public HaveARideCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }
        
        public override void AddTriggers()
        {
            // Reduce damage dealt to hero targets by environment cards by 1
            AddReduceDamageTrigger(damage => damage.DamageSource.IsEnvironmentSource && damage.Target.IsHero, damage => 1);
        }

        public override System.Collections.IEnumerator Play()
        {
            yield break;
        }
    }
}
