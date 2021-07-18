using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Alexandria
{
    public class InvincibleCardController : CardController
    {
        public InvincibleCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // "From when this card enters play until the start of your next turn, when {AlexandriaCharacter} would take damage you may discard a card. If you do, prevent the damage.",
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
            // "Reduce damage dealt to {AlexandriaCharacter} by 1"
        }
    }
}
