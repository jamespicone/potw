using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Echidna
{
    public class CloneArmyCardController : CardController
    {
        public CloneArmyCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // At the end of the villain turn play the top card of the Twisted deck.
            AddEndOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => GameController.PlayTopCardOfLocation(TurnTakerController, TurnTaker.FindSubDeck("TwistedDeck")),
                TriggerType.PlayCard
            );
        }
    }
}
