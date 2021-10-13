using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Grue
{
    public class SecondTriggerCardController : CardController
    {
        public SecondTriggerCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }
    }
}
