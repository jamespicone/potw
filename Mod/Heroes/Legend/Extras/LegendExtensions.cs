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

            var selectedEffects = new List<IEffectCardController>();

            while (co.Card.IsInPlayAndHasGameText)
            {
                // Other controllers can answer for our card and report our ability as
                // theirs, so offer only the effects that are ours. SelectAndActivateAbility
                // can't do this: it only filters by Card, which is the same for both.
                var choices = co.GameController
                    .GetActivatableAbilitiesInPlayEx(co.HeroTurnTakerController, "effect", cardSource: co.GetCardSource())
                    .Where(a => EffectFor(a) != null && ! selectedEffects.Contains(EffectFor(a)));
                if (! choices.Any()) { break; }

                var decision = new ActivateAbilityDecision(
                    co.GameController,
                    co.HeroTurnTakerController,
                    "effect",
                    choices,
                    optional: selectedEffects.Count() > 0,
                    co.GetCardSource()
                );

                var e = co.GameController.MakeDecisionAction(decision);
                if (co.UseUnityCoroutines) { yield return co.GameController.StartCoroutine(e); }
                else { co.GameController.ExhaustCoroutine(e); }

                if (! decision.Completed || decision.SelectedAbility == null) { break; }

                e = co.GameController.ActivateAbility(decision.SelectedAbility, co.GetCardSource());
                if (co.UseUnityCoroutines) { yield return co.GameController.StartCoroutine(e); }
                else { co.GameController.ExhaustCoroutine(e); }

                selectedEffects.Add(EffectFor(decision.SelectedAbility));

                if (! areWeBursting) { break; }
            }

            effects.AddRange(selectedEffects);
        }

        private static IEffectCardController EffectFor(ActivatableAbility ability)
        {
            return (ability?.CopiedFromCardController ?? ability?.CardController) as IEffectCardController;
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
