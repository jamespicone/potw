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
    public class ToughCardController : ThugCardController
    {
        public ToughCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP(ranking: 1, numberOfTargets: 2);
        }

        public override void AddDamageTriggers()
        {
            // At the end of the villain turn, this card deals the 2 hero targets with the highest HP 2 melee damage each.
            AddDealDamageAtEndOfTurnTrigger(
                TurnTaker,
                Card,
                c => c.Is(this).Hero().Target(),
                TargetType.HighestHP,
                2,
                DamageType.Melee,
                numberOfTargets: 2
            );
        }
    }
}
