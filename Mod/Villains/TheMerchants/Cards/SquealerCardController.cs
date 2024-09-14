using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.TheMerchants
{
    public class SquealerCardController : CardController
    {
        public SquealerCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // Villain targets are immune to damage from environment cards
            AddImmuneToDamageTrigger(dda => dda.DamageSource.Is().Environment() && dda.Target.Is(this).Villain().Target());

            // At the start of the villain turn, play the top card of the environment deck.
            AddStartOfTurnTrigger(tt => tt == TurnTaker, PlayTheTopCardOfTheEnvironmentDeckResponse, TriggerType.PlayCard);
        }
    }
}
