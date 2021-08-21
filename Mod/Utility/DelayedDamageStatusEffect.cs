using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Jp.ParahumansOfTheWormverse.Utility
{
    [Serializable]
    class DelayedDamageStatusEffect : OnPhaseChangeStatusEffect
    {
        public DelayedDamageStatusEffect(Card cardWithMethod, string nameOfMethod, string description, Card cardSource)
            : base(cardWithMethod, nameOfMethod, description, new TriggerType[] { TriggerType.DealDamage }, cardSource)
        {
        }

        public void DealDamageToTargetAtStartOfNextTurn(TurnTaker player, Card target, int damage)
        {
            DamageToDeal = damage;

            UntilTargetLeavesPlay(target);

            TurnTakerCriteria.IsSpecificTurnTaker = player;
            TurnPhaseCriteria.Phase = Phase.Start;
            BeforeOrAfter = BeforeOrAfter.After;
            CanEffectStack = true;
        }

        public int DamageToDeal { get; private set; }
    }

    public static class DelayedDamageExtension
    {
        public static IEnumerator DoDelayedDamage(this CardController co, OnPhaseChangeStatusEffect effect)
        {
            IEnumerator e;
            while (true)
            {
                var delayedDamageEffect = effect as DelayedDamageStatusEffect;
                if (delayedDamageEffect == null) { break; }

                var target = delayedDamageEffect.TargetLeavesPlayExpiryCriteria.IsOneOfTheseCards.FirstOrDefault();
                if (target == null) { break; }

                if (co.CharacterCard.IsIncapacitatedOrOutOfGame || ! target.IsTarget || ! target.IsInPlayAndNotUnderCard) { break; }
                if (co.GameController.IsCardVisibleToCardSource(target, co.GetCardSource(delayedDamageEffect)))
                {
                    e = co.GameController.DealDamageToTarget(
                        new DamageSource(co.GameController, co.CharacterCard),
                        target,
                        delayedDamageEffect.DamageToDeal,
                        DamageType.Projectile,
                        cardSource: co.GetCardSource()
                    );
                    if (co.UseUnityCoroutines)
                    {
                        yield return co.GameController.StartCoroutine(e);
                    }
                    else
                    {
                        co.GameController.ExhaustCoroutine(e);
                    }
                }
                else
                {
                    e = co.GameController.SendMessageAction(
                        $"{target.Title} is no longer visible",
                        Priority.Medium,
                        co.GetCardSource(),
                        new[] { target },
                        showCardSource: true
                    );
                    if (co.UseUnityCoroutines)
                    {
                        yield return co.GameController.StartCoroutine(e);
                    }
                    else
                    {
                        co.GameController.ExhaustCoroutine(e);
                    }
                }

                break;
            }

            e = co.GameController.ExpireStatusEffect(effect, co.GetCardSource());
            if (co.UseUnityCoroutines)
            {
                yield return co.GameController.StartCoroutine(e);
            }
            else
            {
                co.GameController.ExhaustCoroutine(e);
            }
        }
    }
}
