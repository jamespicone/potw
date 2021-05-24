using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Slaughterhouse9
{
    public class TheSiberianCharacterCardController : Slaughterhouse9MemberCharacterCardController
    {
        public TheSiberianCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        public override void AddSideTriggers()
        {
            // Card PlayIndex is when they came into play

            if (Card.IsFlipped)
            {
                // "The villain target with the lowest HP is immune to melee damage"
                AddSideTrigger(AddTrigger<DealDamageAction>(
                    dda => dda.DamageType == DamageType.Melee &&
                        dda.Target.IsVillainTarget &&
                        CanCardBeConsideredLowestHitPoints(dda.Target, c => c.IsVillainTarget),
                    dda => MaybeImmune(dda),
                    TriggerType.ImmuneToDamage,
                    TriggerTiming.Before
                ));
            }
            else
            {
                // "The Nine target played just before and just after The Siberian are immune to damage"
                AddSideTrigger(AddImmuneToDamageTrigger(
                    dda =>
                        dda.Target.IsTarget &&
                        dda.Target.DoKeywordsContain("nine") &&
                        IsAdjacentNine(dda.Target)
                ));
            }
        }

        private bool IsAdjacentNine(Card target)
        {
            var nine = FindCardsWhere(new LinqCardCriteria(c => c.IsInPlayAndNotUnderCard && c.IsTarget && c.DoKeywordsContain("nine")), GetCardSource());
            nine = nine.Where(c => c.PlayIndex != null && c.PlayIndex != Card.PlayIndex).OrderBy(c => c.PlayIndex);
            
            if (target.PlayIndex < Card.PlayIndex)
            {
                nine = nine.Where(c => c.PlayIndex < Card.PlayIndex);
                return nine.Last() == target;
            }
            else
            {
                nine = nine.Where(c => c.PlayIndex > Card.PlayIndex);
                return nine.First() == target;
            }
        }

        private IEnumerator MaybeImmune(DealDamageAction dda)
        {
            if (dda.IsPretend) { yield break; }

            var wasLowest = new List<bool>();
            var e = DetermineIfGivenCardIsTargetWithLowestOrHighestHitPoints(
                dda.Target,
                highest: false,
                c => c.IsVillainTarget,
                dda,
                wasLowest
            );

            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            if (wasLowest.Count > 0 && wasLowest.First())
            {
                e = GameController.ImmuneToDamage(dda, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }
            }
        }
    }
}
