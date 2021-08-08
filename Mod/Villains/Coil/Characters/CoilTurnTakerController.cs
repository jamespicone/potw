using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Jp.ParahumansOfTheWormverse.Coil
{
    public class CoilTurnTakerController : TurnTakerController
    {
        public CoilTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        { }

        public override IEnumerator StartGame()
        {
            var scheming = TurnTaker.GetCardByIdentifier("CoilSchemingCharacter");
            var acting = TurnTaker.GetCardByIdentifier("CoilActingCharacter");
            var ubase = TurnTaker.GetCardByIdentifier("UndergroundBase");

            var e1 = GameController.MoveIntoPlay(this, scheming, TurnTaker);
            var e2 = GameController.MoveIntoPlay(this, acting, TurnTaker);
            var e3 = GameController.MoveIntoPlay(this, ubase, TurnTaker);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e1);
                yield return GameController.StartCoroutine(e2);
                yield return GameController.StartCoroutine(e3);
            }
            else
            {
                GameController.ExhaustCoroutine(e1);
                GameController.ExhaustCoroutine(e2);
                GameController.ExhaustCoroutine(e3);
            }
        }
    }
}
