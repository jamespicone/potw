using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Grue
{
    public class HideAndRecoverCardController : CardController
    {
        public HideAndRecoverCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }
    }
}
