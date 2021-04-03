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
    public class KnockEmDownCardController : BatteryUtilityCardController
    {
        public KnockEmDownCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            ShowBatteryChargedStatus();
        }

        public override IEnumerator Play()
        {
            // "{BatteryCharacter} deals 1 non-hero target 2 melee damage."
            IEnumerator singleCoroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), 2, DamageType.Melee, 1, false, 1, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(singleCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(singleCoroutine);
            }
            // "If {BatteryCharacter} is {Charged}, she deals each non-hero target 2 melee damage."
            if (IsBatteryCharged())
            {
                IEnumerator massCoroutine = base.GameController.DealDamage(base.HeroTurnTakerController, base.CharacterCard, (Card c) => c.IsTarget && !c.IsHero && base.GameController.IsCardVisibleToCardSource(c, GetCardSource()), 2, DamageType.Melee, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(massCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(massCoroutine);
                }
            }
            yield break;
        }
    }
}
