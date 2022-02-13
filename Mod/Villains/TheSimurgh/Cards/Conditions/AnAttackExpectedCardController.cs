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
    public class AnAttackExpectedCardController : ConditionCardController
    {
        public AnAttackExpectedCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        protected override bool IsConditionMet()
        {
            // If {TheSimurghCharacter} was dealt damage last round
            return Journal.DealDamageEntriesToTargetSinceLastTurn(
                CharacterCard,
                TurnTaker
            ).Where(ddje => ddje.Amount > 0).Count() > 0;
        }
    }
}
