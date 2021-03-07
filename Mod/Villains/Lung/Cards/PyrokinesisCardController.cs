using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Lung
{
    public class PyrokinesisCardController : CardController
    {
        public PyrokinesisCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override System.Collections.IEnumerator Play()
        {
            // Whenever {Lung} deals melee damage to a target, {Lung} deals 2 fire damage to that target
            yield break;
        }
    }
}
