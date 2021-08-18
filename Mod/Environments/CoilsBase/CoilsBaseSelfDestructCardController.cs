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
    public class CoilsBaseSelfDestructCardController : CardController
    {
        public CoilsBaseSelfDestructCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "At the start of their turn, a player may skip the rest of their turn to destroy this card."
            AddStartOfTurnTrigger((TurnTaker tt) => tt.IsHero, base.SkipTheirTurnToDestroyThisCardResponse, new TriggerType[] { TriggerType.SkipTurn, TriggerType.DestroySelf });
        }
    }
}
