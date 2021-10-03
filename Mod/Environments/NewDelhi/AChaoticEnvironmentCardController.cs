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
    public class AChaoticEnvironmentCardController : CardController
    {
        public AChaoticEnvironmentCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "At the end of the environment turn, play the top card of the environment deck."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, base.PlayTheTopCardOfTheEnvironmentDeckResponse, TriggerType.PlayCard);
            // "At the start of the environment turn, if this is the only environment card in play, destroy it."
            AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => base.GameController.DestroyCard(DecisionMaker, base.Card, responsibleCard: base.Card, cardSource: GetCardSource()), TriggerType.DestroySelf, (PhaseChangeAction pca) => FindCardsWhere((Card c) => c.IsInPlay && c.Is().Environment() && c != base.Card, visibleToCard: GetCardSource()).Count() <= 0);
            base.AddTriggers();
        }
    }
}
