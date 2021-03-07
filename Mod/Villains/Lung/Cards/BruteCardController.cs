using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Lung
{
    public class BruteCardController : CardController
    {
        public BruteCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }
    }
}
