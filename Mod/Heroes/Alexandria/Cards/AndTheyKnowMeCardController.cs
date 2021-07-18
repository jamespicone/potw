using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Alexandria
{
    public class AndTheyKnowMeCardController : CardController
    {
        public AndTheyKnowMeCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // The villain target with the highest HP deals {AlexandriaCharacter} 5 psychic damage
            // The villain target with the highest HP cannot deal damage until the start of your next turn
            yield break;

            //if (UseUnityCoroutines)
            //{
            //    yield return GameController.StartCoroutine(e);
            //}
            //else
            //{
            //    GameController.ExhaustCoroutine(e);
            //}
        }
    }
}
