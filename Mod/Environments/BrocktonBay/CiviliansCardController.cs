using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.BrocktonBay
{
    public class CiviliansCardController : CardController
    {
        public CiviliansCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "Players cannot use powers."
            CannotUsePowers((TurnTakerController ttc) => true);
            // "At the start of the environment turn, each player may discard 1 card each to destroy this card."
            AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => EachPlayerMayDiscardOneCardToPerformAction(pca, base.GameController.DestroyCard(DecisionMaker, base.Card, responsibleCard: base.Card, cardSource: GetCardSource()), "destroy " + base.Card.Title), new TriggerType[] { TriggerType.DiscardCard, TriggerType.DestroySelf });
            base.AddTriggers();
        }
    }
}
