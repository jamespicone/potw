using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Kyushu
{
    public class OnlyTheIndomitableRemainCardController : CardController
    {
        public OnlyTheIndomitableRemainCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "Reduce all damage by 1."
            AddReduceDamageTrigger((Card c) => true, 1);
            // "At the end of the environment turn, each target regains 1 HP."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => base.GameController.GainHP(DecisionMaker, (Card c) => c.IsTarget, 1, cardSource: GetCardSource()), TriggerType.GainHP);
            // "When a target is destroyed, destroy this card."
            AddTrigger<DestroyCardAction>((DestroyCardAction dca) => dca.WasCardDestroyed && dca.CardToDestroy.Card.IsTarget, (DestroyCardAction dca) => base.GameController.DestroyCard(DecisionMaker, base.Card, actionSource: dca, responsibleCard: base.Card, associatedCards: dca.CardToDestroy.Card.ToEnumerable().ToList(), cardSource: GetCardSource()), TriggerType.DestroySelf, TriggerTiming.After);
            base.AddTriggers();
        }
    }
}
