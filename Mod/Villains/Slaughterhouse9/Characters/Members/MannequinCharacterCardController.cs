using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Slaughterhouse9
{
    public class MannequinCharacterCardController : Slaughterhouse9MemberCharacterCardController
    {
        public MannequinCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        public override bool AllowFastCoroutinesDuringPretend => IsLowestHitPointsUnique((c) => c.Alignment(this).Villain().Target());

        public override void AddSideTriggers()
        {
            if (Card.IsFlipped)
            {
                // "Reduce damage dealt to the villain target with the lowest HP by 1"
                reduceDamageTrigger = AddTrigger<DealDamageAction>(
                    dda => dda.Target.Alignment(this).Villain().Target() && CanCardBeConsideredLowestHitPoints(dda.Target, c => c.Alignment(this).Villain().Target()),
                    MaybeReduceDamage,
                    TriggerType.ReduceDamage,
                    TriggerTiming.Before
                );

                AddSideTrigger(reduceDamageTrigger);
            }
            else
            {
                // "Reduce damage dealt to Mannequin by 1"
                AddSideTrigger(AddReduceDamageTrigger(c => c == Card, 1));

                // "The first time a Defence card would enter the trash this turn destroy 1 hero Ongoing or Equipment card"
                AddSideTrigger(AddDefenceTrigger(() => DestroyHeroCard(), new TriggerType[] { TriggerType.DestroyCard }, "MannequinDestroyCard"));
            }
        }

        public IEnumerator MaybeReduceDamage(DealDamageAction dda)
        {
            if (GameController.PretendMode || reduceDamage == null)
            {
                var wasLowest = new List<bool>();
                var e = DetermineIfGivenCardIsTargetWithLowestOrHighestHitPoints(
                    dda.Target,
                    highest: false,
                    c => c.Alignment(this).Villain().Target(),
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

                reduceDamage = wasLowest.Count() > 0 && wasLowest.First();
            }

            if (reduceDamage.GetValueOrDefault(false))
            {
                var e = GameController.ReduceDamage(dda, 1, reduceDamageTrigger, GetCardSource());
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
                reduceDamage = null;
            }
        }

        public IEnumerator DestroyHeroCard()
        {
            return GameController.SelectAndDestroyCard(
                DecisionMaker,
                new LinqCardCriteria(c => c.Alignment().Hero() && (c.IsOngoing || c.DoKeywordsContain("equipment")), "Hero Ongoing or Equipment"),
                optional: false,
                responsibleCard: Card,
                cardSource: GetCardSource()
            );
        }

        private ITrigger reduceDamageTrigger;
        private bool? reduceDamage;
    }
}
