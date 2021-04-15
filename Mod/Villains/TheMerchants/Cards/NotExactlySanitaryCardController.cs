using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.TheMerchants
{
    public class NotExactlySanitaryCardController : TheMerchantsUtilityCardController
    {
        public NotExactlySanitaryCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "Whenever a villain target deals non-toxic damage to another target, that villain target also deals that target 1 toxic damage."
            AddTrigger<DealDamageAction>((DealDamageAction dda) => dda.DamageSource != null && dda.DamageSource.Card.IsVillainTarget && dda.Target != null && dda.Target != dda.DamageSource.Card && dda.DidDealDamage && dda.DamageType != DamageType.Toxic, ToxicDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
            base.AddTriggers();
        }

        public IEnumerator ToxicDamageResponse(DealDamageAction dda)
        {
            // "... that villain target also deals that target 1 toxic damage."
            Card villainTarget = dda.DamageSource.Card;
            Card damagedTarget = dda.Target;
            IEnumerator toxicCoroutine = DealDamage(villainTarget, damagedTarget, 1, DamageType.Toxic, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(toxicCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(toxicCoroutine);
            }
            yield break;
        }
    }
}
