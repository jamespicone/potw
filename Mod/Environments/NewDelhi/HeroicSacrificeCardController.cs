using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.NewDelhi
{
    public class HeroicSacrificeCardController : CardController
    {
        public HeroicSacrificeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "Whenever damage is redirected from one hero target to another, increase that damage by 2."
            AddTrigger<RedirectDamageAction>((RedirectDamageAction rda) => rda.NewTarget.Is().Hero().Target() && rda.OldTarget.Is().Hero().Target(), IncreaseDamageResponse, TriggerType.IncreaseDamage, TriggerTiming.Before);

            // "When a hero target deals damage to another hero target, destroy this card."
            AddTrigger<DealDamageAction>(
                (DealDamageAction dda) => dda.DidDealDamage &&
                    dda.DamageSource != null &&
                    dda.DamageSource.Card != null &&
                    dda.DamageSource.Is().Hero().Target() &&
                    dda.Target.Is().Hero().Target() &&
                    dda.Target != dda.DamageSource.Card,
                base.DestroyThisCardResponse,
                TriggerType.DestroySelf,
                TriggerTiming.After
            );
        }

        public IEnumerator IncreaseDamageResponse(RedirectDamageAction rda)
        {
            // "... increase that damage by 2."
            IEnumerator increaseCoroutine = base.GameController.IncreaseDamage(rda.DealDamageAction, 2, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(increaseCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(increaseCoroutine);
            }
        }
    }
}
