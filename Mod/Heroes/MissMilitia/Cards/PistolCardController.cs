using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.MissMilitia
{
    public class PistolCardController : WeaponCardController
    {
        public PistolCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController, WeaponType.Pistol)
        {
            ShowWeaponStatusIfActive(WeaponType.SubmachineGun);
        }

        protected override IEnumerator DoWeaponEffect(bool activateAll)
        {
            int amount = GetPowerNumeral(0, 2);

            // "{MissMilitiaCharacter} deals a non-hero target 2 projectile damage."
            var e = GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, CharacterCard),
                amount,
                DamageType.Projectile,
                numberOfTargets: 1,
                optional: false,
                requiredTargets: 1,
                additionalCriteria: (c) => this.HasAlignment(c, CardAlignment.Nonhero, CardTarget.Target),
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

            // "{smg} You may play a card."
            if (activateAll || this.ShouldActivateWeaponAbility(WeaponType.SubmachineGun))
            {
                e = GameController.SelectAndPlayCardFromHand(
                    HeroTurnTakerController,
                    optional: true,
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
