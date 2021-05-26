using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dragon
{
    public class ParahumansOnlineCardController : CardController
    {
        public ParahumansOnlineCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }
    }
}
