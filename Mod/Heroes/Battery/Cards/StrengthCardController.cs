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
    public class StrengthCardController : BatteryUtilityCardController
    {
        public StrengthCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            ShowBatteryChargedStatus();
        }

        public override IEnumerator Play()
        {
            // "{BatteryCharacter} deals 1 target 3 melee damage."
            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            IEnumerator firstDamageCoroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), 3, DamageType.Melee, 1, false, 1, storedResultsDamage: storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(firstDamageCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(firstDamageCoroutine);
            }
            // "If {BatteryCharacter} is {Charged}, she deals the same target 2 more melee damage."
            if (IsBatteryCharged())
            {
                DealDamageAction dd = storedResults.FirstOrDefault();
                if (dd != null && dd.Target != null)
                {
                    IEnumerator additionalDamageCoroutine = base.GameController.DealDamage(base.HeroTurnTakerController, base.CharacterCard, (Card c) => c == dd.Target, 2, DamageType.Melee, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(additionalDamageCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(additionalDamageCoroutine);
                    }
                }
            }
            yield break;
        }
    }
}
