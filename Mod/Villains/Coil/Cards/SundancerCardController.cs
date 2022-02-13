using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Coil
{
    public class SundancerCardController : CardController
    {
        public SundancerCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"This card is immune to fire damage",
            AddImmuneToDamageTrigger(dda => dda.Target == Card && dda.DamageType == DamageType.Fire);

            //"At the end of the villain turn this card deals 2 fire damage to all hero targets"
            AddDealDamageAtEndOfTurnTrigger(
                TurnTaker,
                Card,
                c => c.Is().Hero().Target(),
                TargetType.All,
                2,
                DamageType.Fire
            );
        }
    }
}
