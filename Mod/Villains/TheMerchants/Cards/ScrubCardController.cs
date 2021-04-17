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
    public class ScrubCardController : TheMerchantsUtilityCardController
    {
        public ScrubCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowListOfCardsInPlay(new LinqCardCriteria((Card c) => c.IsTarget && c.HitPoints.Value % 3 == 0 && c != base.Card, "other than " + base.Card.Title + " with HP evenly divisible by 3", useCardsSuffix: false, useCardsPrefix: true));
        }

        public override void AddTriggers()
        {
            // "At the start of the villain turn, this card deals 5 energy damage to each target other than itself whose HP is evenly divisible by 3."
            AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => DealDamage(base.Card, (Card c) => c.HitPoints.Value % 3 == 0 && c != base.Card, (Card c) => 5, DamageType.Energy), TriggerType.DealDamage);
            base.AddTriggers();
        }
    }
}
