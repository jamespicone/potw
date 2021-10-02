using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Lung
{
    public class WingsCardController : CardController
    {
        public WingsCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // {Lung} is immune to damage from Environment cards
            AddImmuneToDamageTrigger(dda => dda.DamageSource.Is().Environment() && dda.Target == CharacterCard);
        }
    }
}
