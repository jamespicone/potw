using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.BrocktonBay
{
    public class PRTSquadCardController : CardController
    {
        public PRTSquadCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNonEnvironmentTargetWithLowestHP(1, 1);
        }

        public override void AddTriggers()
        {
            // "At the end of the environment turn, this card deals the non-environment target with the lowest HP 2 projectile damage."
            AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c.Is().NonEnvironment().Target(), TargetType.LowestHP, 2, DamageType.Projectile, highestLowestRanking: 1, numberOfTargets: 1);
            base.AddTriggers();
        }
    }
}
