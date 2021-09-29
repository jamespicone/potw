
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

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
                dda => dda.DamageType == DamageType.Psychic && this.HasAlignment(dda.Target, CardAlignment.Hero, CardTarget.Target),
                dda => 1
            );
        }
    }
}
