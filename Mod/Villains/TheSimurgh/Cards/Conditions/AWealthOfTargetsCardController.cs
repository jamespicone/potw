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
    public class AWealthOfTargetsCardController : ConditionCardController
    {
        public AWealthOfTargetsCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        protected override bool IsConditionMet()
        {
            // If there are at least {H} non-character card targets in play
            return FindCardsWhere(c => c.IsInPlay && c.IsTarget && ! c.IsCharacter).Count() >= H;
        }
    }
}
