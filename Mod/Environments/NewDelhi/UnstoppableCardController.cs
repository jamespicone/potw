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
    public class UnstoppableCardController : CardController
    {
        private ITrigger ReduceDamageTrigger;

        private bool? PerformReduce
        {
            get;
            set;
        }

        public UnstoppableCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowVillainTargetWithHighestHPIfNotVillain(ranking: 1, numberOfTargets: 1);
            AllowFastCoroutinesDuringPretend = false;
            RunModifyDamageAmountSimulationForThisCard = false;
        }

        public override void AddTriggers()
        {
            // "Reduce damage dealt to the villain target with the highest HP by 1."
            ReduceDamageTrigger = AddTrigger((DealDamageAction dda) => CanCardBeConsideredHighestHitPoints(dda.Target, (Card c) => c.Alignment(this).Villain().Target()), MaybeReduceDamageResponse, TriggerType.ReduceDamage, TriggerTiming.Before);
            // "When the villain target with the highest HP is dealt 4 or more damage at once, destroy this card."
            AddTrigger<DealDamageAction>((DealDamageAction dda) => CanCardBeConsideredHighestHitPoints(dda.Target, (Card c) => c.Alignment(this).Villain().Target()) && dda.Amount >= 4, (DealDamageAction dda) => base.GameController.DestroyCard(DecisionMaker, base.Card, actionSource: dda, responsibleCard: dda.CardSource.Card, cardSource: GetCardSource()), TriggerType.DestroySelf, TriggerTiming.After);
            base.AddTriggers();
        }

        public IEnumerator MaybeReduceDamageResponse(DealDamageAction dda)
        {
            // If target is the villain target with the highest HP, reduce damage by 1
            if (base.GameController.PretendMode)
            {
                List<bool> results = new List<bool>();
                IEnumerator checkCoroutine = DetermineIfGivenCardIsTargetWithLowestOrHighestHitPoints(dda.Target, true, (Card c) => c.Alignment(this).Villain().Target(), dda, results);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(checkCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(checkCoroutine);
                }
                PerformReduce = results.Count() > 0 && results.First();
            }
            if (PerformReduce.HasValue && PerformReduce.Value)
            {
                IEnumerator reduceCoroutine = base.GameController.ReduceDamage(dda, 1, ReduceDamageTrigger, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(reduceCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(reduceCoroutine);
                }
            }
            if (!base.GameController.PretendMode)
            {
                PerformReduce = null;
            }
            yield break;
        }
    }
}
