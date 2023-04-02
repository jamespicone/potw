using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Echidna
{
    public class HubrisTwistedCardController : CardController
    {
        public HubrisTwistedCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowHasBeenUsedThisTurn(
                "HubrisPreventedDamage",
                "{0} has prevented damage this turn",
                "{0} has not prevented damage this turn"
            );
        }

        public override void AddTriggers()
        {
            // At the end of the villain turn this card deals 1 psychic damage to all hero targets
            AddDealDamageAtEndOfTurnTrigger(
                TurnTaker,
                Card,
                c => c.Is(this).Hero().Target(),
                TargetType.All,
                1,
                DamageType.Psychic
            );

            // Prevent the first damage dealt to this card each round
            AddPreventDamageTrigger(
                dda => dda.Target == Card && !HasBeenSetToTrueThisRound("HubrisPreventedDamage"),
                dda => SetProperty(dda),
                followUpTriggerTypes: new TriggerType[] { TriggerType.Hidden },
                isPreventEffect: true
            );
        }

        private IEnumerator SetProperty(DealDamageAction dda)
        {
            SetCardPropertyToTrueIfRealAction("HubrisPreventedDamage", gameAction: dda);
            yield break;
        }
    }
}
