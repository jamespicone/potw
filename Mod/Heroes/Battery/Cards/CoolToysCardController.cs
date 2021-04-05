using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Battery
{
    public class CoolToysCardController : BatteryUtilityCardController
    {
        public CoolToysCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            ShowBatteryChargedStatus();
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && (c.DoKeywordsContain("equipment") || c.DoKeywordsContain("device")), "Equipment or Device"));
        }

        public override IEnumerator Play()
        {
            // "If {BatteryCharacter} is {Charged}, she deals 1 target X lightning damage, where X is the total number of Equipment and Device cards in play."
            if (IsCharged(base.CharacterCard))
            {
                IEnumerator damageCoroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), FindCardsWhere(new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && (c.DoKeywordsContain("equipment") || c.DoKeywordsContain("device")), "Equipment or Device"), GetCardSource()).Count(), DamageType.Lightning, 1, false, 1, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(damageCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(damageCoroutine);
                }
            }
            // "{Discharge} {BatteryCharacter}."
            IEnumerator dischargeCoroutine = RemoveCharge(base.CharacterCard);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(dischargeCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(dischargeCoroutine);
            }
            yield break;
        }
    }
}
