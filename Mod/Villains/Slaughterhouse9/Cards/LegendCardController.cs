using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Slaughterhouse9
{
    public class LegendCardController : CardController
    {
        public LegendCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // Increase damage dealt by villain cards by 1
            AddIncreaseDamageTrigger(dda => dda.DamageSource.IsVillain, 1);
        }
    }
}
