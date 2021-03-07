using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Lung
{
    public class SmashCardController : CardController
    {
        public SmashCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override System.Collections.IEnumerator Play()
        {
            // "{Lung} deals the hero target with the lowest HP 2 melee damage.",
            // "Destroy 1 hero ongoing or equipment card"
            yield break;
        }
    }
}
