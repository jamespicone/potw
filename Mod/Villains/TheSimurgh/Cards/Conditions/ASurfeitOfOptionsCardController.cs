using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.TheSimurgh
{
    public class ASurfeitOfOptionsCardController : ConditionCardController
    {
        public ASurfeitOfOptionsCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        protected override bool IsConditionMet()
        {
            // If there are at least {H} Ongoing cards in play
            return FindCardsWhere(c => c.IsInPlay && IsOngoing(c)).Count() >= H;
        }
    }
}
