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
    public class ShallWePlayAGameCardController : CardController
    {
        public ShallWePlayAGameCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            // "If Jack Slash is not in play, find Jack Slash under the Slaughterhouse 9 card and put him into the villain play area.
            
            // Jack Slash deals 2 psychic damage to all hero targets
            yield break;
        }
    }
}
