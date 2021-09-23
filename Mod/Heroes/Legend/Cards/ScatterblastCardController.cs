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

        public IEnumerator DoEffect(IEnumerable<Card> targets, CardSource cardSource, EffectTargetingOrdering ordering)
        {
            // Legend deals X energy damage, where X = 5 - the number of targets affected, minimum 1
            return this.HandleEffectOrdering(
                targets,
                ordering,
                t => GameController.DealDamageToTarget(
                    new DamageSource(GameController, CharacterCard),
                    t,
                    c => Math.Max(1, 5 - targets.Count()),
                    DamageType.Energy,
                    cardSource: cardSource
                ),
                ts => GameController.SelectTargetsAndDealDamage(
                    HeroTurnTakerController,
                    new DamageSource(GameController, CharacterCard),
                    amount: c => Math.Max(1, 5 - targets.Count()),
                    damageType: DamageType.Energy,
                    dynamicNumberOfTargets: () => ts.Count(),
                    requiredTargets: ts.Count(),
                    optional: false,                    
                    additionalCriteria: c => ts.Contains(c),
                    cardSource: cardSource
                )
            );
        }
    }
}
