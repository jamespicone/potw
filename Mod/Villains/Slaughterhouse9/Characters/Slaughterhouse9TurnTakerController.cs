using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Jp.ParahumansOfTheWormverse.Slaughterhouse9
{
    class Slaughterhouse9TurnTakerController : TurnTakerController
    {
        public Slaughterhouse9TurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        { }

        public override IEnumerator StartGame()
        {
            yield break;
        }
    }
}
