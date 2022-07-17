using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Echidna
{
    public class EchidnaTurnTakerController : TurnTakerController
    {
        public EchidnaTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {
        }

        public override IEnumerator StartGame()
        {
            // Put the top {H - 2} cards of the Twisted deck into play.
            return GameController.PlayTopCardOfLocation(this, TurnTaker.FindSubDeck("TwistedDeck"), numberOfCards: H - 2);
        }
    }
}
