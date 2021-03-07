using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Jp.ParahumansOfTheWormverse.Lung
{
    class LungTurnTakerController : TurnTakerController
    {
        public LungTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        { }

        public override IEnumerator StartGame()
        {
            return base.StartGame();
        }
    }
}
