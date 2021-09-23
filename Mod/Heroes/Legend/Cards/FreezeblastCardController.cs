using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Legend
{
    public class FreezeblastCardController : CardController, IEffectCardController
    {
        public FreezeblastCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public DealDamageAction TypicalDamageAction(IEnumerable<Card> targets)
        {
            return new DealDamageAction(
                GetCardSource(),
                new DamageSource(GameController, CharacterCard),
                null,
                1,
                DamageType.Cold,
                addStatusEffect: dda => ReduceDamageDealtByThatTargetUntilTheStartOfYourNextTurnResponse(dda, 1)
            );
        }

        public IEnumerator DoEffect(IEnumerable<Card> targets, CardSource cardSource, EffectTargetingOrdering ordering)
        {
            // Legend deals 1 cold damage. Targets dealt damage in this way deal 1 less damage until the start of your next turn
            return this.HandleEffectOrdering(
                targets,
                ordering,
                t => GameController.DealDamageToTarget(
                    new DamageSource(GameController, CharacterCard),
                    t,
                    1,
                    DamageType.Cold,
                    addStatusEffect: dda => ReduceDamageDealtByThatTargetUntilTheStartOfYourNextTurnResponse(dda, 1),
                    cardSource: cardSource
                ),
                ts => GameController.DealDamage(
                    HeroTurnTakerController,
                    CharacterCard,
                    c => ts.Contains(c),
                    1,
                    DamageType.Cold,
                    addStatusEffect: dda => ReduceDamageDealtByThatTargetUntilTheStartOfYourNextTurnResponse(dda, 1),
                    cardSource: cardSource
                )
            );
        }
    }
}
