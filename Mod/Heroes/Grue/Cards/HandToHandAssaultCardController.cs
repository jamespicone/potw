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
    public class HandToHandAssaultCardController : CardController
    {
        public HandToHandAssaultCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // {GrueCharacter} deals 2 melee damage to a non-hero target.
            var e = GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, CharacterCard),
                2,
                DamageType.Melee,
                additionalCriteria: c => c.Is().NonHero().Target(),
                numberOfTargets: 1,
                optional: false,
                requiredTargets: 1,
                addStatusEffect: dda => DealExtraDamageIfDarkness(dda),
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

        private IEnumerator DealExtraDamageIfDarkness(DealDamageAction dda)
        {
            // If that target has a Darkness card next to it {GrueCharacter} deals that target another 2 melee damage
            if (!this.DoesTargetHaveDarknessAdjacent(dda.Target))
            {
                yield break;
            }

            var e = GameController.SelectAndDestroyCard(
                HeroTurnTakerController,
                new LinqCardCriteria(c => c.IsOngoing, "ongoing"),
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
