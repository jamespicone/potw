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
    public class MacheteCardController : WeaponCardController
    {
        public MacheteCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController, "{machete}")
        {
            ShowWeaponStatusIfActive(SubmachineGunKey);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int firstAmount = GetPowerNumeral(0, 2);
            int secondAmount = GetPowerNumeral(1, 3);
            List<SelectCardDecision> targetChoice = new List<SelectCardDecision>();
            Func<Card, IEnumerable<DealDamageAction>> followUp = (Card c) => ActivateWeaponEffectForPower(SubmachineGunKey) ? new DealDamageAction[] { new DealDamageAction(GetCardSource(), new DamageSource(base.GameController, base.CharacterCard), c, secondAmount, DamageType.Melee) } : null;
            // "{MissMilitiaCharacter} deals a non-hero target 2 melee damage."
            //IEnumerator firstDamageCoroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.Card), (Card c) => firstAmount, DamageType.Melee, () => 1, false, 1, additionalCriteria: (Card c) => !c.IsHero, storedResultsDecisions: targetChoice, cardSource: GetCardSource(), dynamicFollowUpDamageInformation: followUp);
            IEnumerator firstDamageCoroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), firstAmount, DamageType.Melee, 1, false, 1, additionalCriteria: (Card c) => !c.IsHero, storedResultsDecisions: targetChoice, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(firstDamageCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(firstDamageCoroutine);
            }
            // "{smg} {MissMilitiaCharacter} deals that target 3 melee damage."
            if (ActivateWeaponEffectForPower(SubmachineGunKey) && targetChoice != null && targetChoice.FirstOrDefault() != null && targetChoice.FirstOrDefault().SelectedCard != null)
            {
                Card target = targetChoice.FirstOrDefault().SelectedCard;
                IEnumerator secondDamageCoroutine = DealDamage(base.CharacterCard, target, secondAmount, DamageType.Melee, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(secondDamageCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(secondDamageCoroutine);
                }
            }
            yield break;
        }
    }
}
