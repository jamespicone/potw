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
    public class IDontSleepCardController : MissMilitiaUtilityCardController
    {
        public IDontSleepCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "At the end of your turn, draw a card."
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DrawCardResponse, TriggerType.DrawCard);
            base.AddTriggers();
        }

        public IEnumerator DrawCardResponse(PhaseChangeAction pca)
        {
            // "... draw a card."
            IEnumerator drawCoroutine = base.GameController.DrawCard(base.HeroTurnTaker, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(drawCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(drawCoroutine);
            }
            yield break;
        }
    }
}
