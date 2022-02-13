using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.MissMilitia
{
    public class CoverFireCardController : MissMilitiaUtilityCardController
    {
        public CoverFireCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            // "{MissMilitiaCharacter} deals a non-hero target 2 projectile damage."
            var e = GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, CharacterCard),
                amount: 2,
                DamageType.Projectile,
                numberOfTargets: 1,
                optional: false,
                requiredTargets: 1,
                additionalCriteria: (c) => c.Is().NonHero().Target(),
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

            // "1 hero target regains 2 HP."
            e = GameController.SelectAndGainHP(
                HeroTurnTakerController,
                amount: 2,
                optional: false,
                (c) => c.Is().Hero().Target(),
                numberOfTargets: 1,
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
