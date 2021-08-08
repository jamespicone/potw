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
    public class MercenariesCardController : CardController
    {
        public MercenariesCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // "At the end of the villain turn this card deals {H - 1} energy damage to the two hero targets with the highest HP"
            AddDealDamageAtEndOfTurnTrigger(
                TurnTaker,
                Card,
                c => c.IsHeroTarget(),
                TargetType.HighestHP,
                H - 1,
                DamageType.Energy,
                numberOfTargets: 2
            );
        }
    }
}
