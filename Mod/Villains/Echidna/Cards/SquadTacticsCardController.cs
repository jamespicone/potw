using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Echidna
{
    public class SquadTacticsCardController : CardController
    {
        public SquadTacticsCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // Increase damage dealt by villain targets by 1.
            AddIncreaseDamageTrigger(
                dda => dda.DamageSource.Is().Villain().Target().AccordingTo(this),
                1
            );
        }
    }
}
