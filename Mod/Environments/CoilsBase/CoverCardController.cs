using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.CoilsBase
{
    public class CoverCardController : CardController
    {
        public CoverCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // Redirect any damage that would be dealt by a nonenvironment source to a target other than this target to this card.
            AddRedirectDamageTrigger(dda => dda.DamageSource.Is().NonEnvironment() && dda.Target != Card, () => Card);

            // "Reduce damage dealt to this card by 1."
            AddReduceDamageTrigger(c => c == Card, 1);
        }
    }
}
