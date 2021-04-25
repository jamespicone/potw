using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Slaughterhouse9
{
    public class WeDontTakePrisonersCardController : CardController
    {
        public WeDontTakePrisonersCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // Whenever a target is destroyed by a villain card remove it from the game
            // TODO: Ugh this is a pain to implement. Unforgiving Wasteland does a lot of manual work.
            // Also it's a bit weak
        }
    }
}
