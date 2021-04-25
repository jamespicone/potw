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
    public class TheyreAllBetterNowCardController : CardController
    {
        public TheyreAllBetterNowCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            // Flip the most recently defeated Nine card and set their HP to 10. If no cards are flipped in this way play the top card of the villain deck
            yield break;
        }
    }
}
