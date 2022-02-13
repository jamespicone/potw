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
    public class ADefencePenetratedCardController : CardController
    {
        public ADefencePenetratedCardController(Card card, TurnTakerController controller) : base(card, controller)
        {}

        public override void AddTriggers()
        {
            // Damage that would be dealt to hero targets cannot be redirected.
            AddMakeDamageNotRedirectableTrigger(dda => dda.Target.Is().Hero().Target());
        }
    }
}
