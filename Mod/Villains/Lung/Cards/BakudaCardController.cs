using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Lung
{
    public class BakudaCardController : CardController
    {
        public BakudaCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override System.Collections.IEnumerator Play()
        {
            /*
                "At the start of the villain turn, reveal the top card of the villain deck.",
                "If it is a One-Shot, {Bakuda} deals 5 fire damage to all non-villain targets.",
                "If it is an Ongoing, each hero discards 1 card.",
                "If it is a target, the hero with the most cards in play destroys all of them",
                "Shuffle the revealed card back into the villain deck"
            */
            yield break;
        }
    }
}
