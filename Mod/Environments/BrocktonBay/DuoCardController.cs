using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.BrocktonBay
{
    public class DuoCardController : CardController
    {
        protected const string LeetIdentifier = "Leet";
        protected const string UberIdentifier = "Uber";

        public DuoCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }
    }
}
