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
            : base(card, turnTakerController, "{pistol}")
        {
            ShowWeaponStatusIfActive(SubmachineGunKey);
        }

        public override IEnumerator UsePower(int index = 0)
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
                additionalCriteria: (c) => ! c.IsHeroTarget(),
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
            if (ActivateWeaponEffectForPower(SubmachineGunKey))
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
