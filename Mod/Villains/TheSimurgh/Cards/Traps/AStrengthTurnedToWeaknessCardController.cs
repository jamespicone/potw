using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.TheSimurgh
{
    public class AStrengthTurnedToWeaknessCardController : CardController
    {
        public AStrengthTurnedToWeaknessCardController(Card card, TurnTakerController controller) : base(card, controller)
        {}

        public override void AddTriggers()
        {
            // "Whenever a hero target deals more than {8 - H} damage, that target deals itself 1 melee damage.",
            AddTrigger<DealDamageAction>(
                dda => dda.DamageSource.Is().Hero().Target() && dda.DidDealDamage && dda.Amount > (8 - H),
                dda => SourcePunchesThemselves(dda),
                TriggerType.DealDamage,
                TriggerTiming.After
            );
        }

        private IEnumerator SourcePunchesThemselves(DealDamageAction dda)
        {
            return DealDamage(
                dda.DamageSource.Card,
                dda.DamageSource.Card,
                1,
                DamageType.Melee,
                isCounterDamage: true,
                cardSource: GetCardSource()
            );
        }
    }
}
