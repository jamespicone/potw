using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

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
            AddTrigger<RedirectDamageAction>((RedirectDamageAction rda) => this.HasAlignment(rda.NewTarget, CardAlignment.Hero, CardTarget.Target) && this.HasAlignment(rda.OldTarget, CardAlignment.Hero, CardTarget.Target), IncreaseDamageResponse, TriggerType.IncreaseDamage, TriggerTiming.Before);

            // "When a hero target deals damage to another hero target, destroy this card."
            AddTrigger<DealDamageAction>(
                (DealDamageAction dda) => dda.DidDealDamage &&
                    dda.DamageSource != null &&
                    dda.DamageSource.Card != null &&
                    this.HasAlignment(dda.DamageSource.Card, CardAlignment.Hero, CardTarget.Target) &&
                    this.HasAlignment(dda.Target, CardAlignment.Hero, CardTarget.Target) &&
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
