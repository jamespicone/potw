using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Tattletale
{
    public class MyEyeOnYouCardController : CardController
    {
        public MyEyeOnYouCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            // "One hero target regains 3 HP."
            int amount = GetPowerNumeral(0, 3);
            IEnumerator healCoroutine = base.GameController.SelectAndGainHP(base.HeroTurnTakerController, amount, additionalCriteria: (Card c) => c.Is(this).Hero().Target(), cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(healCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(healCoroutine);
            }
        }
    }
}
