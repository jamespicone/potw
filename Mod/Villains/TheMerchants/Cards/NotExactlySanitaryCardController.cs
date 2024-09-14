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
    public class NotExactlySanitaryCardController : CardController
    {
        public NotExactlySanitaryCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // Whenever a villain target deals non-toxic damage to another target, that villain target also deals that target 1 toxic damage."
            AddTrigger<DealDamageAction>(
                dda => dda.DamageSource.Is(this).Villain().Target() &&
                    dda.DamageSource.IsCard && 
                    dda.Target != dda.DamageSource.Card &&
                    dda.DidDealDamage &&
                    dda.DamageType != DamageType.Toxic,
                ToxicDamageResponse,
                TriggerType.DealDamage,
                TriggerTiming.After
            );
        }

        public IEnumerator ToxicDamageResponse(DealDamageAction dda)
        {
            // "... that villain target also deals that target 1 toxic damage."
            var e = DealDamage(dda.DamageSource.Card, dda.Target, 1, DamageType.Toxic, cardSource: GetCardSource());
            if (UseUnityCoroutines) yield return GameController.StartCoroutine(e);
            else GameController.ExhaustCoroutine(e);
        }
    }
}
