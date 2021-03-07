using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Lung
{
    public class TerribleBurnsCardController : CardController
    {
        public TerribleBurnsCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override System.Collections.IEnumerator Play()
        {
            // "Whenever a hero is dealt fire damage by {Lung}, destroy 1 hero ongoing or equipment card.",
            // "At the end of the villain turn, {Lung} deals the hero target with the highest HP {H - 2} fire damage"
            yield break;
        }
    }
}
