using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Leviathan
{
    public class ImpossibleStrengthCardController : CardController
    {
        public ImpossibleStrengthCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }
        
        public override void AddTriggers()
        {
            // Increase damage dealt by villain cards by 1
            AddIncreaseDamageTrigger((dda) => dda.DamageSource != null && dda.DamageSource.IsVillain, 1);
        }
    }
}
