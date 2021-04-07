using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.MissMilitia
{
    public class PistolCardController : WeaponCardController
    {
        public PistolCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController, "{pistol}")
        {
            ShowWeaponStatusIfActive(SubmachineGunKey);
        }

        public override void AddTriggers()
        {
            base.AddTriggers();
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int amount = GetPowerNumeral(0, 2);
            // "{MissMilitiaCharacter} deals a non-hero target 2 projectile damage."
            IEnumerator damageCoroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), amount, DamageType.Projectile, 1, false, 1, additionalCriteria: (Card c) => !c.IsHero, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(damageCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(damageCoroutine);
            }
            // "{smg} You may play a card."
            if (ActivateWeaponEffectForPower(SubmachineGunKey))
            {
                IEnumerator playCoroutine = base.GameController.SelectAndPlayCardFromHand(base.HeroTurnTakerController, true, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(playCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(playCoroutine);
                }
            }
            yield break;
        }
    }
}
