using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Alexandria
{
    public class ProtectorCardController : CardController
    {
        public ProtectorCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // "When this card enters play {AlexandriaCharacter} regains 2 HP",
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
            // "At the end of your turn select a target. Until the start of your next turn, whenever that target would take damage, redirect that damage to {AlexandriaCharacter}"
        }
    }
}
