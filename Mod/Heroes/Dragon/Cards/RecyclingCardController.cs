using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dragon
{
    public class RecyclingCardController : CardController
    {
        public RecyclingCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }
    }
}
