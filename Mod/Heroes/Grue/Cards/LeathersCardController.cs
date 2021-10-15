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
    public class LeathersCardController : CardController
    {
        public LeathersCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // Reduce non-psychic damage dealt to {GrueCharacter} by 1
            AddReduceDamageTrigger(dda => dda.Target == CharacterCard && dda.DamageType != DamageType.Psychic, dda => 1);
        }
    }
}
