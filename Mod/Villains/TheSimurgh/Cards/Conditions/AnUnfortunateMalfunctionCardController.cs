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
    public class AnUnfortunateMalfunctionCardController : ConditionCardController
    {
        public AnUnfortunateMalfunctionCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        protected override bool IsConditionMet()
        {
            // If there are at least {H} Equipment cards in play
            return FindCardsWhere(c => c.IsInPlay && c.DoKeywordsContain("equipment")).Count() >= H;
        }
    }
}
