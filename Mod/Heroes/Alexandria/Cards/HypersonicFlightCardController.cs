using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Alexandria
{
    public class HypersonicFlightCardController : CardController
    {
        public HypersonicFlightCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // "When this card enters play {AlexandriaCharacter} may deal 2 melee damage to a target",
            //if (UseUnityCoroutines)
            //{
            //    yield return GameController.StartCoroutine(e);
            //}
            //else
            //{
            //    GameController.ExhaustCoroutine(e);
            //}
            yield break;
        }

        public override void AddTriggers()
        {
            // "When {AlexandriaCharacter} deals damage to a villain target you may destroy a Device or Ongoing card"
        }
    }
}
