using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Lung
{
    public class GrowingAngerCardController : CardController
    {
        public GrowingAngerCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override System.Collections.IEnumerator Play()
        {
            // Discard the top 3 cards of the villain deck. Play the top card of the villain deck
            yield break;
        }
    }
}
