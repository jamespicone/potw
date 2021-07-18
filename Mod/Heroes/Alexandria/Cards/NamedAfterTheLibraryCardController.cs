using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Alexandria
{
    public class NamedAfterTheLibraryCardController : CardController
    {
        public NamedAfterTheLibraryCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // "When this card enters play shuffle your trash into your deck",
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
            // "Whenever you would draw a card instead reveal the top 2 cards of your deck, put 1 into your hand, and return the other to the top of your deck"
        }
    }
}
