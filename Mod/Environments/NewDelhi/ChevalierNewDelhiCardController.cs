using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.NewDelhi
{
    public class ChevalierNewDelhiCardController : CardController
    {
        public ChevalierNewDelhiCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "Reduce damage dealt to this card by 1."
            AddReduceDamageTrigger((Card c) => c == base.Card, 1);
            // "Whenever damage would be dealt to a hero, redirect it to this card."
            AddRedirectDamageTrigger((DealDamageAction dda) => dda.Target.Is().Hero().Target().Character(), () => base.Card);
            base.AddTriggers();
        }
    }
}
