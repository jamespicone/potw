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
    public class PushingTheLimitsCardController : BatteryUtilityCardController
    {
        public PushingTheLimitsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            ShowBatteryChargedStatus();
        }

        public override IEnumerator Play()
        {
            // "If {BatteryCharacter} is {Charged}, you may play up to 2 cards."
            if (IsBatteryCharged())
            {
                IEnumerator playCoroutine = base.GameController.SelectAndPlayCardsFromHand(base.HeroTurnTakerController, 2, true, 0, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(playCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(playCoroutine);
                }
            }
            // "If {BatteryCharacter} is {Discharged}, draw 3 cards."
            if (!IsBatteryCharged())
            {
                IEnumerator drawCoroutine = base.GameController.DrawCards(base.HeroTurnTakerController, 3, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(drawCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(drawCoroutine);
                }
            }
            yield break;
        }
    }
}
