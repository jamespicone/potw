using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Grue
{
    public class BlindingCardController : CardController
    {
        public BlindingCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // "{GrueCharacter} deals 2 melee damage to a non-hero target.",
            var e = GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, CharacterCard),
                2,
                DamageType.Melee,
                numberOfTargets: 1,
                optional: false,
                requiredTargets: 1,
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

            // "If you deal damage to a target with a Darkness card next to it you may destroy an Ongoing card"
            yield break;
        }
    }
}
