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
    public class AMillionCoincidencesCardController : CardController, ISimurghDangerCard
    {
        public AMillionCoincidencesCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public int Danger()
        {
            // TODO
            return 0;
        }

        public override IEnumerator Play()
        {
            // "The Simurgh deals 1 projectile damage to all hero targets H-1 times.",
            for (int i = 0; i < H - 1; ++i)
            {
                var e = GameController.DealDamage(
                    DecisionMaker,
                    CharacterCard,
                    c => c.Is().Hero().Target(),
                    1,
                    DamageType.Projectile,
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
            }

            // "The environment deals 1 irreducible melee damage to all hero targets H-2 times."
            for (int i = 0; i < H - 2; ++i)
            {
                var e = GameController.SelectTargetsAndDealDamage(
                    DecisionMaker,
                    new DamageSource(GameController, FindEnvironment().TurnTaker),
                    1,
                    DamageType.Melee,
                    additionalCriteria: c => c.Is().Hero().Target(),
                    numberOfTargets: null,
                    optional: false,
                    requiredTargets: null,
                    isIrreducible: true,
                    allowAutoDecide: true,
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
            }
        }
    }
}
