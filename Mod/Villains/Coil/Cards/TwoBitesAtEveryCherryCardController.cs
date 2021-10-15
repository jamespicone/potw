using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Coil
{
    public class TwoBitesAtEveryCherryCardController : CardController
    {
        public TwoBitesAtEveryCherryCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"Increase damage dealt by villain targets by 1",
            AddIncreaseDamageTrigger(dda => dda.DamageSource.Is(this).Villain().Target(), 1);

            //"Damage dealt by villain targets is irreducible"
            AddMakeDamageIrreducibleTrigger(dda => dda.DamageSource.Is(this).Villain().Target());
        }
    }
}
