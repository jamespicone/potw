using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Armsmaster
{
    public class ReturnInAFlashCardController : CardController
    {
        public ReturnInAFlashCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }
    }
}
