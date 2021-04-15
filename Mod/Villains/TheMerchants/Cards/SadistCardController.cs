using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.TheMerchants
{
    public class SadistCardController : TheMerchantsUtilityCardController
    {
        public SadistCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithLowestHP(ranking: 1, numberOfTargets: 1);
        }

        public override void AddTriggers()
        {
            // "At the end of the villain turn, this card deals the hero target with the lowest HP 2 melee damage."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => DealDamageToLowestHP(base.Card, 1, (Card c) => c.IsHero, (Card c) => 2, DamageType.Melee), TriggerType.DealDamage);
            base.AddTriggers();
        }
    }
}
