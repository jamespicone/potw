using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Legend
{
    public class RelativisticCardController : CardController
    {
        public RelativisticCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // "You may play a card",
            // "You may play a card"
            var e = SelectAndPlayCardsFromHand(
                HeroTurnTakerController,
                numberOfCards: 2,
                requiredDecisions: 0
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }
    }
}
