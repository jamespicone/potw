using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Skitter
{
    public class ImpeccableAimCardController : CardController
    {
        public ImpeccableAimCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator UsePower(int index = 0)
        {
            if (index == 0)
            {
                return ShootAToddler();
            }
            else
            {
                return DestroyAnEnvironmentCard();
            }            
        }

        private IEnumerator DestroyAnEnvironmentCard()
        {
            // "Destroy an environment card."
            var e = GameController.SelectAndDestroyCard(
                HeroTurnTakerController,
                new LinqCardCriteria(c => c.Is().Environment(), "environment"),
                optional: false,
                responsibleCard: CharacterCard,
                cardSource: GetCardSource()
            );

            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }

        private IEnumerator ShootAToddler()
        {
            // "{SkitterCharacter} deals a target 2 irreducible projectile damage.
            var results = new List<DealDamageAction>();
            var e = GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, CharacterCard),
                GetPowerNumeral(0, 2),
                DamageType.Projectile,
                1,
                optional: false,
                requiredTargets: 1,
                isIrreducible: true,
                storedResultsDamage: results,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // {SkitterCharacter} may deal another target 1 projectile damage."
            e = GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, CharacterCard),
                GetPowerNumeral(1, 1),
                DamageType.Projectile,
                1,
                optional: false,
                additionalCriteria: c => !results.Any(dda => dda.OriginalTarget == c),
                requiredTargets: 0,
                storedResultsDamage: results,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
