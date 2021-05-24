using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            base.AddTriggers();
            // "Redirect any damage that would be dealt to a target other than this target to this card."
            AddRedirectDamageTrigger((DealDamageAction dda) => dda.Target != base.Card, () => base.Card);
            // "Reduce damage dealt to this card by 1."
            AddReduceDamageTrigger((Card c) => c == base.Card, 1);
        }
    }
}
