using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.Legend
{
    public static class LegendExtensions
    {
        public static IEnumerator ChooseEffects(this CardController co, List<IEffectCardController> effects)
        {
            var areWeBursting = co.GameController.StatusEffectManager.StatusEffectControllers.Select(sec => sec.StatusEffect as LegendBurstStatusEffect)
                .Where(se => se != null && se.AffectedTurnTaker == co.TurnTaker).Count() > 0;

            var selectedEffects = new List<CardController>();

            do
            {
                var selectedEffect = new List<ActivateAbilityDecision>();
                var e = co.GameController.SelectAndActivateAbility(
                    co.HeroTurnTakerController,
                    "effect",
                    new LinqCardCriteria(c => selectedEffects.Count(cc => cc.Card == c) <= 0),
                    storedResults: selectedEffect,
                    cardSource: co.GetCardSource(),
                    optional: selectedEffects.Count() > 0
                );
                if (co.UseUnityCoroutines)
                {
                    yield return co.GameController.StartCoroutine(e);
                }
                else
                {
                    co.GameController.ExhaustCoroutine(e);
                }

                if (selectedEffect.Count() <= 0) { break; }

                var selected = selectedEffect.First().SelectedAbility?.CopiedFromCardController ?? selectedEffect.First().SelectedAbility?.CardController;
                if (selected == null) { break; }

                selectedEffects.Add(selected);
            } while (areWeBursting);

            effects.AddRange(selectedEffects.Cast<IEffectCardController>());
        }

        public static IEnumerator ApplyEffects(
            this CardController co,
            IEnumerable<IEffectCardController> effects,
            IEnumerable<Card> targets,
            EffectTargetingOrdering ordering,
            CardSource cardSourceToUse
        )
        {
            // apply effects
            foreach (var effect in effects)
            {
                var e = effect.DoEffect(targets, co, cardSourceToUse, ordering);
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

        public static IEnumerator HandleEffectOrdering(
            this CardController co,
            IEnumerable<Card> targets,
            EffectTargetingOrdering ordering,
            Func<Card, IEnumerator> singleTargetVersion,
            Func<IEnumerable<Card>, IEnumerator> multiTargetVersion
        )
        {
            if (ordering == EffectTargetingOrdering.NeedsOrdering)
            {
                var e = multiTargetVersion(targets);
                if (co.UseUnityCoroutines)
                {
                    yield return co.GameController.StartCoroutine(e);
                }
                else
                {
                    co.GameController.ExhaustCoroutine(e);
                }
            }

            if (ordering == EffectTargetingOrdering.OrderingAlreadyDecided)
            {
                foreach (Card c in targets)
                {
                    var e = singleTargetVersion(c);
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
    }
}
