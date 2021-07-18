using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Alexandria
{
    public class MartialArtsCardController : CardController
    {
        public MartialArtsCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // "When this card enters play you may destroy a non-character-card target",
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
            // "When {AlexandriaCharacter} destroys a target select a target. Until the start of your next turn that target cannot deal damage"
        }
    }
}
