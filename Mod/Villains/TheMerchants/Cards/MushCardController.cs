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
    public class MushCardController : CardController
    {
        public MushCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // "This card is immune to melee and projectile damage."
            AddImmuneToDamageTrigger(
                dda => dda.Target == Card &&
                    (dda.DamageType == DamageType.Melee || dda.DamageType == DamageType.Projectile)
            );

            // "At the end of the villain turn, this card deals each hero target 1 melee damage."
            AddDealDamageAtEndOfTurnTrigger(
                TurnTaker,
                Card,
                c => c.Is().Hero().Target().AccordingTo(this),
                TargetType.All,
                1,
                DamageType.Melee
            );
        }
    }
}
