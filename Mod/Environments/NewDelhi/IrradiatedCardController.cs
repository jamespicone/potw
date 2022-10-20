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
    public class IrradiatedCardController : CardController
    {
        private const string PoolIdentifier = "IrradiatedPool";

        public IrradiatedCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowTokenPool(base.Card.FindTokenPool(PoolIdentifier));
        }

        public override void AddTriggers()
        {
            // "At the end of the environment turn, put a token on this card."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => base.GameController.AddTokensToPool(base.Card.FindTokenPool(PoolIdentifier), 1, GetCardSource()), TriggerType.AddTokensToPool);
            // "At the start of the environment turn, deal each target X energy damage, where X is the number of tokens on this card. If there are no tokens on this card, destroy it."
            AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DamageOrDestroyResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.DestroyCard });
            // "Whenever a target regains HP, remove a token from this card."
            AddTrigger<GainHPAction>((GainHPAction gha) => gha.AmountActuallyGained > 0, (GainHPAction gha) => base.GameController.RemoveTokensFromPool(base.Card.FindTokenPool(PoolIdentifier), 1, cardSource: GetCardSource()), TriggerType.ModifyTokens, TriggerTiming.After);
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

        public IEnumerator DamageOrDestroyResponse(PhaseChangeAction pca)
        {
            // "... deal each target X energy damage, where X is the number of tokens on this card."
            IEnumerator damageCoroutine = base.GameController.DealDamage(DecisionMaker, base.Card, (Card c) => true, base.Card.FindTokenPool(PoolIdentifier).CurrentValue, DamageType.Energy, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(damageCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(damageCoroutine);
            }
            if (base.Card.FindTokenPool(PoolIdentifier).CurrentValue == 0)
            {
                // "If there are no tokens on this card, destroy it."
                IEnumerator destroyCoroutine = base.GameController.DestroyCard(DecisionMaker, base.Card, responsibleCard: base.Card, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(destroyCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(destroyCoroutine);
                }
            }
            yield break;
        }
    }
}
