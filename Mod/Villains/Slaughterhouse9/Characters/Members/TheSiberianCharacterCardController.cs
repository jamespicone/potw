﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Slaughterhouse9
{
    public class TheSiberianCharacterCardController : Slaughterhouse9MemberCharacterCardController
    {
        public TheSiberianCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        public override bool AllowFastCoroutinesDuringPretend => IsLowestHitPointsUnique((c) => c.Is(this).Villain().Target());

        public override void AddSideTriggers()
        {
            // Card PlayIndex is when they came into play

            if (Card.IsFlipped)
            {
                // "The villain target with the lowest HP is immune to melee damage"
                AddSideTrigger(AddTrigger<DealDamageAction>(
                    dda => dda.DamageType == DamageType.Melee &&
                        dda.Target.Is(this).Villain().Target() &&
                        CanCardBeConsideredLowestHitPoints(dda.Target, c => c.Is(this).Villain().Target()),
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
                return nine.LastOrDefault() == target;
            }
            else
            {
                nine = nine.Where(c => c.PlayIndex > Card.PlayIndex);
                return nine.FirstOrDefault() == target;
            }
        }

        private IEnumerator MaybeImmune(DealDamageAction dda)
        {
            if (GameController.PretendMode || preventDamage == null)
            {

                var wasLowest = new List<bool>();
                var e = DetermineIfGivenCardIsTargetWithLowestOrHighestHitPoints(
                    dda.Target,
                    highest: false,
                    c => c.Is(this).Villain().Target(),
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

                preventDamage = wasLowest.Count > 0 && wasLowest.First();
            }

            if (preventDamage.GetValueOrDefault(false))
            {
                var e = GameController.ImmuneToDamage(dda, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }
            }

            if (! GameController.PretendMode)
            {
                preventDamage = null;
            }
        }

        private bool? preventDamage;
    }
}
