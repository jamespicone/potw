using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.TheMerchants
{
    public class FestivalOfBloodCardController : CardController
    {
        public FestivalOfBloodCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // "Increase damage dealt by villain targets by 1."
            AddIncreaseDamageTrigger((dda) => dda.DamageSource.Is(this).Villain().Target(), (dda) => 1);
        }
    }
}
