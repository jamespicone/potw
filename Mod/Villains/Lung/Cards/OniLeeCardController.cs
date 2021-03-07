using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Lung
{
    public class OniLeeCardController : CardController
    {
        public OniLeeCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override System.Collections.IEnumerator Play()
        {
            // "At the end of the villain turn, {Oni Lee} deals the hero target with the lowest HP 3 melee damage.",
            // "Whenever {Oni Lee} would take damage, reveal the top card of the villain deck. If it is a one-shot prevent the damage. Shuffle the revealed card back into the villain deck"
            yield break;
        }
    }
}
