using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.NewDelhi
{
    public class PhirSeCardController : CardController
    {
        private const string PoolIdentifier = "PhirSePool";

        public PhirSeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowTokenPool(base.Card.Identifier, PoolIdentifier);
            SpecialStringMaker.ShowHighestHP(ranking: 1, numberOfTargets: () => 1, new LinqCardCriteria((Card c) => c.IsInPlay && c.IsTarget));
        }

        public override void AddTriggers()
        {
            // "At the end of the environment turn, move this card next to the target with the highest HP."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, MoveNextToResponse, TriggerType.MoveCard);
            // "At the start of the environment turn, put 2 tokens on this card."
            AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => base.GameController.AddTokensToPool(base.Card.FindTokenPool(PoolIdentifier), 2, GetCardSource()), TriggerType.AddTokensToPool);
            // "When this card is destroyed, it deals X energy damage to the target it is next to, where X is the number of tokens on this card."
            AddWhenDestroyedTrigger(GoForthAndCleanResponse, TriggerType.DealDamage);
            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            // Enters play with 0 tokens
            IEnumerator resetCoroutine = ResetTokenPool();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(resetCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(resetCoroutine);
            }
        }

        private IEnumerator ResetTokenPool()
        {
            var pool = Card.FindTokenPool(PoolIdentifier);
            if (pool == null) { yield break; }

            pool.SetToInitialValue();
            yield break;
        }

        public IEnumerator MoveNextToResponse(PhaseChangeAction pca)
        {
            // "... move this card next to the target with the highest HP."
            List<Card> highestResults = new List<Card>();
            IEnumerator findCoroutine = base.GameController.FindTargetWithHighestHitPoints(1, (Card c) => true, highestResults, evenIfCannotDealDamage: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(findCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(findCoroutine);
            }
            Card highest = highestResults.FirstOrDefault();
            if (highest != null)
            {
                IEnumerator moveCoroutine = base.GameController.MoveCard(base.TurnTakerController, base.Card, highest.NextToLocation, responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(moveCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(moveCoroutine);
                }
            }
            yield break;
        }

        public IEnumerator GoForthAndCleanResponse(DestroyCardAction dca)
        {
            // "... it deals X energy damage to the target it is next to, where X is the number of tokens on this card."
            IEnumerator damageCoroutine = DealDamage(base.Card, GetCardThisCardIsNextTo(), base.Card.FindTokenPool(PoolIdentifier).CurrentValue, DamageType.Energy, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(damageCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(damageCoroutine);
            }
            yield break;
        }
    }
}
