using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Jp.ParahumansOfTheWormverse.Lung
{
    public class LungTurnTakerController : TurnTakerController
    {
        public LungTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        { }

        public override IEnumerator StartGame()
        {
            var brute = TurnTaker.GetCardByIdentifier("BruteInstructions");
            var e = GameController.MoveIntoPlay(this, brute, TurnTaker);
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
