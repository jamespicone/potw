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
    public class HailOfBulletsCardController : MissMilitiaUtilityCardController
    {
        public HailOfBulletsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            // "{MissMilitiaCharacter} deals each non-hero target 2 projectile damage."
            var e = DealDamage(
                CharacterCard,
                (c) => c.Is(this).NonHero().Target(),
                amount: 2,
                DamageType.Projectile
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
