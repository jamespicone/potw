using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Skitter
{
    public class VindictiveCreativityCardController : CardController
    {
        public VindictiveCreativityCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            AllowFastCoroutinesDuringPretend = false;
        }

        public override void AddTriggers()
        {
            // "Whenever the swarm deals damage to a villain target, increase that damage by 1.",
            AddIncreaseDamageTrigger(
                dda => dda.Target.Is().Villain().Target().AccordingTo(this) && this.IsSwarmDamage(dda),
                1
            );

            // "Whenever the swarm deals damage, change the damage type to a type of your choice."
            AddTrigger(
                new ChangeDamageTypeTrigger(
                    GameController,
                    dda => this.IsSwarmDamage(dda),
                    SelectDamageTypeForDealDamageAction,
                    new TriggerType[1] { TriggerType.ChangeDamageType },
                    possibleDamageTypes: null,
                    cardSource: GetCardSource()
                )
            );
        }
    }
}
