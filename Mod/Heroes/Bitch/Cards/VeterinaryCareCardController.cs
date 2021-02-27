using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Bitch
{
    public class VeterinaryCareCardController : CardController
    {
        public VeterinaryCareCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override System.Collections.IEnumerator Play()
        {
            yield break;
        }
    }
}
