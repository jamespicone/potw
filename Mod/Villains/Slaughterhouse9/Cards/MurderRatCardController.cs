using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Slaughterhouse9
{
    public class MurderRatCardController : CardController
    {
        public MurderRatCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // At the end of the villain turn Murder Rat deals 2 melee damage to the hero target with the lowest HP
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, c => c.IsHero && c.IsTarget, TargetType.LowestHP, 2, DamageType.Melee);
        }
    }
}
