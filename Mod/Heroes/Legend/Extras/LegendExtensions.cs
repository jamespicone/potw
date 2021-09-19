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
        public static IEnumerator SelectAndPerformEffects(this CardController co, IEnumerable<Card> targets)
        {
            var areWeBursting = co.GameController.StatusEffectManager.StatusEffectControllers.Select(sec => sec.StatusEffect as LegendBurstStatusEffect)
                .Where(se => se != null && se.AffectedTurnTaker == co.TurnTaker ).Count() > 0;

            var selectedEffects = new List<CardController>();

            do
            {
                var selectedEffect = new List<ActivateAbilityDecision>();
                var e = co.GameController.SelectAndActivateAbility(
                    co.HeroTurnTakerController,
                    "effect",
                    new LinqCardCriteria(c => selectedEffects.Count(cc => cc.Card == c) <= 0),
                    storedResults: selectedEffect,
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

                if (selectedEffect.Count() <= 0) { break; }

                var selected = selectedEffect.First().SelectedAbility?.CardController;
                if (selected == null) { break; }

                selectedEffects.Add(selected);
            } while (areWeBursting);

            // apply effects
            foreach (var effect in selectedEffects)
            {
                var effectIntf = effect as IEffectCardController;
                if (effectIntf == null) { continue; }

                var e = effectIntf.DoEffect(targets);
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
