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
    public class RapidReconCardController : BatteryUtilityCardController
    {
        public RapidReconCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            ShowBatteryChargedStatus();
        }

        public override IEnumerator Play()
        {
            // "If {BatteryCharacter} is {Discharged}, each player may draw a card."
            if (!IsBatteryCharged())
            {
                IEnumerator massDrawCoroutine = EachPlayerDrawsACard(optional: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(massDrawCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(massDrawCoroutine);
                }
            }
            // "Draw a card."
            IEnumerator singleDrawCoroutine = base.GameController.DrawCard(base.HeroTurnTaker, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(singleDrawCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(singleDrawCoroutine);
            }
            // "You may play a card."
            IEnumerator playCoroutine = base.GameController.SelectAndPlayCardFromHand(base.HeroTurnTakerController, true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(playCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(playCoroutine);
            }
            yield break;
        }
    }
}
