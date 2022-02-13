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
    public class MacheteCardController : WeaponCardController
    {
        public MacheteCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController, WeaponType.Machete)
        {
            ShowWeaponStatusIfActive(WeaponType.SubmachineGun);
        }

        protected override IEnumerator DoWeaponEffect(bool activateAll)
        {
            int firstAmount = GetPowerNumeral(0, 2);
            int secondAmount = GetPowerNumeral(1, 3);
            var targetChoice = new List<SelectCardDecision>();

            // "{MissMilitiaCharacter} deals a non-hero target 2 melee damage."
            var e = GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, CharacterCard),
                amount: firstAmount,
                DamageType.Melee,
                numberOfTargets: 1,
                optional: false,
                requiredTargets: 1,
                additionalCriteria: (c) => c.Is().NonHero().Target(),
                storedResultsDecisions: targetChoice,
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

            // "{smg} {MissMilitiaCharacter} deals that target 3 melee damage."
            if ((activateAll || this.ShouldActivateWeaponAbility(WeaponType.SubmachineGun)) && targetChoice?.FirstOrDefault()?.SelectedCard != null)
            {
                e = DealDamage(
                    CharacterCard,
                    targetChoice.FirstOrDefault().SelectedCard,
                    secondAmount,
                    DamageType.Melee,
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
