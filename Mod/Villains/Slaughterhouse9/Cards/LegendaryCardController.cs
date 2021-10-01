using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Slaughterhouse9
{
    public class LegendaryCardController : CardController
    {
        public LegendaryCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // Increase damage dealt by villain cards by 1
            AddIncreaseDamageTrigger(dda => dda.DamageSource.Alignment(this).Villain(), 1);
        }
    }
}
