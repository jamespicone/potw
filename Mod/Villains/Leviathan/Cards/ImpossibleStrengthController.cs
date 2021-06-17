using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Leviathan
{
    public class ImpossibleStrengthController : CardController
    {
        public ImpossibleStrengthController(Card card, TurnTakerController controller) : base(card, controller)
        { }
        
        public override void AddTriggers()
        {
            // Increase damage dealt by villain cards by 1
            AddIncreaseDamageTrigger((DealDamageAction dda) => dda.DamageSource != null && IsVillainTarget(dda.DamageSource.Card), 1);
        }

        public override System.Collections.IEnumerator Play()
        {
            yield break;
        }
    }
}
