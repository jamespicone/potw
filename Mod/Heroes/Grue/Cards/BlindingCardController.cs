using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

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
                additionalCriteria: c => c.Is(this).NonHero().Target(),
                numberOfTargets: 1,
                optional: false,
                requiredTargets: 1,
                addStatusEffect: dda => DestroyOngoingIfTargetDarknessed(dda),
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

        private IEnumerator DestroyOngoingIfTargetDarknessed(DealDamageAction dda)
        {
            // "If you deal damage to a target with a Darkness card next to it you may destroy an Ongoing card"
            if (! this.DoesTargetHaveDarknessAdjacent(dda.Target) || ! dda.DidDealDamage)
            {
                yield break;
            }

            var e = GameController.SelectAndDestroyCard(
                HeroTurnTakerController,
                new LinqCardCriteria(c => c.Is(this).Ongoing(), "ongoing"),
                optional: true,
                responsibleCard: Card,
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
