using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.TheSimurgh
{
    public class AReckoningLongDelayedCardController : CardController
    {
        public AReckoningLongDelayedCardController(Card card, TurnTakerController controller) : base(card, controller)
        {}

        public override IEnumerator Play()
        {
            // When this card is flipped face up the hero with the lowest HP deals the hero with the highest HP 5 irreducible projectile damage
            var storedResult = new List<Card>();
            var e = GameController.FindTargetWithLowestHitPoints(
                1,
                c => c.Is().Hero().Target().Character(),
                storedResult,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var source = storedResult.FirstOrDefault();
            if (source == null) { yield break; }

            e = DealDamageToHighestHP(
                source,
                1,
                c => c.Is().Hero().Target().Character(),
                c => 5,
                DamageType.Projectile,
                isIrreducible: true
            );
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