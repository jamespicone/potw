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
    public class SomeMoreFriendsCardController : CardController
    {
        public SomeMoreFriendsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            // Move the top card under the Slaughterhouse 9 card into the villain play area. Play the top card of the villain deck
            yield break;
        }
    }
}
