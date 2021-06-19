using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Leviathan
{
    public class ImpossibleToughnessCardController : CardController
    {
        public ImpossibleToughnessCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }
        
        public override void AddTriggers()
        {
            // Reduce damage dealt to Leviathan by 1
            AddReduceDamageTrigger(c => c == CharacterCard, 1);
        }
    }
}
