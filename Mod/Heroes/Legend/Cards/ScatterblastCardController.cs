using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Legend
{
    public class ScatterblastCardController : CardController, IEffectCardController
    {
        public ScatterblastCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public DealDamageAction TypicalDamageAction(IEnumerable<Card> targets)
        {
            // This is used when a new target hasn't been picked yet, so targets is off by 1
            return new DealDamageAction(
                GetCardSource(),
                new DamageSource(GameController, CharacterCard),
                null,
                c => Math.Max(1, 4 - targets.Count()),
                DamageType.Energy
            );
        }

        public IEnumerator DoEffect(IEnumerable<Card> targets, EffectTargetingOrdering ordering)
        {
            // Legend deals X energy damage, where X = 5 - the number of targets affected, minimum 1
            var e = DealDamage(
                CharacterCard,
                c => targets.Contains(c),
                Math.Max(1, 5 - targets.Count()),
                DamageType.Energy
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
