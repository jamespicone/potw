using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.TheSimurgh
{
    public class ACapabilityRevealedCardController : CardController
    {
        public ACapabilityRevealedCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }
    }
}
