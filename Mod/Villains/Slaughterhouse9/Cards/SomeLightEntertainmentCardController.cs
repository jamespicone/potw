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
    public class SomeLightEntertainmentCardController : CardController
    {
        public SomeLightEntertainmentCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            // Each villain target in play deals 2 melee damage to the hero target with the highest HP
            return this.SelectTargetsToDealDamageToTarget(
                DecisionMaker,
                c => c.IsVillainTarget,
                damageDealer => DealDamageToHighestHP(
                    damageDealer,
                    1,
                    c => c.IsHeroTarget(),
                    c => 2,
                    DamageType.Melee
                )
            );
        }
    }
}
