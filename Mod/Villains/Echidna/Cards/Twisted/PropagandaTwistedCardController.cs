using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Echidna
{
    public class PropagandaTwistedCardController : CardController
    {
        public PropagandaTwistedCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP();

            var pool = Card.FindTokenPool("EnergyPool");
            if (pool != null)
            {
                SpecialStringMaker.ShowTokenPool(pool, this, "There {0} {1} Energy {2} on " + Card.Title);
            }
        }

        public override void AddTriggers()
        {
            // At the end of the villain turn...
            AddEndOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => DoEndOfTurnDamage(),
                new TriggerType[] { TriggerType.DealDamage, TriggerType.ModifyTokens }
            );

            // "Whenever this card takes damage put an Energy token on it."
            AddTrigger<DealDamageAction>(
                dda => dda.Target == Card && dda.DidDealDamage,
                dda => AddTokens(),
                TriggerType.AddTokensToPool,
                TriggerTiming.After
            );
        }

        private IEnumerator DoEndOfTurnDamage()
        {
            // ...this card deals the hero target with the highest HP X irreducible energy damage,
            // where X = 2 + the number of Energy tokens on this card.
            var pool = Card.FindTokenPool("EnergyPool");
            var tokenCount = pool?.CurrentValue ?? 0;

            var e = DealDamageToHighestHP(
                Card,
                1,
                c => c.Is(this).Hero().Target(),
                c => 2 + tokenCount,
                DamageType.Energy,
                isIrreducible: true
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // Then remove all Energy tokens from this card.",
            if (pool != null)
            {
                e = GameController.RemoveTokensFromPool(pool, pool.CurrentValue, cardSource: GetCardSource());
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }

        }

        private IEnumerator AddTokens()
        {
            var pool = Card.FindTokenPool("EnergyPool");
            if (pool == null) { yield break; }

            var e = GameController.AddTokensToPool(pool, 1, GetCardSource());
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
