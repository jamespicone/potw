using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.NewDelhi
{
    public class WildfiresCardController : CardController
    {
        public WildfiresCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "At the end of the environment turn, deal each target 2 fire damage."
            AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c.IsTarget, TargetType.All, 2, DamageType.Fire);
            // "When a target is dealt cold damage, destroy this card."
            AddTrigger<DealDamageAction>((DealDamageAction dda) => dda.DidDealDamage && dda.DamageType == DamageType.Cold, (DealDamageAction dda) => base.GameController.DestroyCard(DecisionMaker, base.Card, actionSource: dda, responsibleCard: base.Card, cardSource: GetCardSource()), TriggerType.DestroySelf, TriggerTiming.After);
            base.AddTriggers();
        }
    }
}
