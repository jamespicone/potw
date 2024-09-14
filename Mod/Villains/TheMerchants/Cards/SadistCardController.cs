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
    public class SadistCardController : ThugCardController
    {
        public SadistCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithLowestHP(ranking: 1, numberOfTargets: 1);
        }

        public override void AddDamageTriggers()
        {
            // "At the end of the villain turn, this card deals the hero target with the lowest HP 2 melee damage."
            AddDealDamageAtEndOfTurnTrigger(
                TurnTaker,
                Card,
                c => c.Is(this).Hero().Target(),
                TargetType.LowestHP,
                2,
                DamageType.Melee
            );
        }
    }
}
