﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Bitch
{
    public class BastardCardController : CardController
    {
        public BastardCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }
    }
}
