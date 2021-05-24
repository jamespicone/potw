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
    public class MercenariesCardController : CardController
    {
        public MercenariesCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHighestHP(ranking: 1, numberOfTargets: () => 2);
        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "At the end of the environment turn, this card deals the 2 targets with the highest HP {H - 1} energy damage each."
            AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => true, TargetType.HighestHP, H - 1, DamageType.Energy, numberOfTargets: 2);
        }
    }
}
