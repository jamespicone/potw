using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Lung
{
    public class BurstOfFlameCardController : CardController
    {
        public BurstOfFlameCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override System.Collections.IEnumerator Play()
        {
            // Lung deals the hero target with the highest HP X fire damage, where X = 1 + the number of cards in the villain trash / 2
            yield break;
        }
    }
}
