using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Leviathan
{
    public class ImpossibleToughnessController : CardController
    {
        public ImpossibleToughnessController(Card card, TurnTakerController controller) : base(card, controller)
        { }
        
        public override void AddTriggers()
        {
            // Reduce damage dealt to Leviathan by 1
            AddReduceDamageTrigger(damage => damage.Target.IsVillainCharacterCard, damage => 1);
        }

        public override System.Collections.IEnumerator Play()
        {
            yield break;
        }
    }
}
