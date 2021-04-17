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
    public class ToughCardController : TheMerchantsUtilityCardController
    {
        public ToughCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP(ranking: 1, numberOfTargets: 2);
        }

        public override void AddTriggers()
        {
            // "At the end of the villain turn, this card deals the 2 hero targets with the highest HP 2 melee damage each."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => DealDamageToHighestHP(base.Card, 1, (Card c) => c.IsHero, (Card c) => 2, DamageType.Melee, numberOfTargets: () => 2), TriggerType.DealDamage);
            base.AddTriggers();
        }
    }
}
