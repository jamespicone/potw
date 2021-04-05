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
    public class ShockAndAweCardController : BatteryUtilityCardController
    {
        public ShockAndAweCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            ShowBatteryChargedStatus();
        }

        public override IEnumerator Play()
        {
            // "{BatteryCharacter} deals 1 target 2 lightning damage."
            List<DealDamageAction> firstStrike = new List<DealDamageAction>();
            IEnumerator firstDamageCoroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), 2, DamageType.Lightning, 1, false, 1, storedResultsDamage: firstStrike, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(firstDamageCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(firstDamageCoroutine);
            }
            // "If {BatteryCharacter} is {Charged}, she deals up to 3 other targets 3 lightning damage each."
            if (IsBatteryCharged())
            {
                Card previouslyDamaged = null;
                if (firstStrike != null)
                {
                    DealDamageAction actualStrike = firstStrike.Where((DealDamageAction dda) => dda.DidDealDamage).FirstOrDefault();
                    if (actualStrike != null)
                    {
                        previouslyDamaged = actualStrike.Target;
                    }
                }
                IEnumerator additionalDamageCoroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), 3, DamageType.Lightning, 3, false, 0, additionalCriteria: (Card c) => c != previouslyDamaged, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(additionalDamageCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(additionalDamageCoroutine);
                }
            }
            yield break;
        }
    }
}
