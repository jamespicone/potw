
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.JessicaYamada
{
    class ResilienceAndRespectCardController : CardController
    {
        public ResilienceAndRespectCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // "Reduce psychic damage dealt to hero targets by 1"
            AddReduceDamageTrigger(
                dda => dda.DamageType == DamageType.Psychic && dda.Target.Is(this).Hero().Target(),
                dda => 1
            );
        }
    }
}
