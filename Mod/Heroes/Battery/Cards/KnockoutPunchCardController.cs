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
    public class KnockoutPunchCardController : BatteryUtilityCardController
    {
        public KnockoutPunchCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            ShowBatteryChargedStatus();
        }

        public override IEnumerator Play()
        {
            // "{BatteryCharacter} deals 1 target 2 lightning damage."
            List<DealDamageAction> damageActions = new List<DealDamageAction>();
            IEnumerator damageCoroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), 2, DamageType.Lightning, 1, false, 1, storedResultsDamage: damageActions, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(damageCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(damageCoroutine);
            }
            // "If {BatteryCharacter} is {Charged}, a non-character card target dealt damage this way loses all text until the start of your next turn."
            if (IsBatteryCharged())
            {
                IEnumerable<Card> validChoices = (from dda in damageActions where dda.DidDealDamage && dda.Target != null select dda.Target).Distinct();
                // ...
            }
            yield break;
        }
    }
}
