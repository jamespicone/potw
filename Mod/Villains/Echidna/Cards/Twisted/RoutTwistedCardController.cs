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
    public class RoutTwistedCardController : CardController
    {
        public RoutTwistedCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            var pool = Card.FindTokenPool("ChargePool");
            if (pool != null)
            {
                SpecialStringMaker.ShowTokenPool(pool, this, "There {0} {1} Charge {2} on " + Card.Title);
            }
        }

        public override void AddTriggers()
        {
            // "At the start of the villain turn...
            AddStartOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => DoStartOfTurnDamage(),
                TriggerType.DealDamage
            );

            // "At the end of the villain turn place a Charge token on this card.",
            AddEndOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => AddTokens(),
                TriggerType.AddTokensToPool
            );
        }

        private IEnumerator DoStartOfTurnDamage()
        {
            // ...this card deals the hero target with the highest HP X energy damage,
            // where X = 2 + the number of Charge tokens on this card."
            var pool = Card.FindTokenPool("ChargePool");
            var tokenCount = pool?.CurrentValue ?? 0;

            var e = DealDamageToHighestHP(
                Card,
                1,
                c => c.Is().Hero().Target(),
                c => 2 + tokenCount,
                DamageType.Energy
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }

        private IEnumerator AddTokens()
        {
            var pool = Card.FindTokenPool("ChargePool");
            if (pool == null) { yield break; }

            var e = GameController.AddTokensToPool(pool, 1, GetCardSource());
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
