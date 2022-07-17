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
    public class SpearpointTwistedCardController : CardController
    {
        public SpearpointTwistedCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // "At the end of the villain turn destroy a hero Equipment card.",
            AddEndOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => GameController.SelectAndDestroyCard(
                    DecisionMaker,
                    new LinqCardCriteria(c => c.Is().Hero().WithKeyword("equipment").AccordingTo(this), "hero equipment"),
                    optional: false,
                    responsibleCard: Card,
                    cardSource: GetCardSource()
                ),
                TriggerType.DestroyCard
            );

            // "Whenever an Equipment card enters play this card regains 5 HP."
            AddTrigger<CardEntersPlayAction>(
                cp => cp.CardEnteringPlay.DoKeywordsContain("equipment"),
                cp => GameController.GainHP(Card, 5, cardSource: GetCardSource()),
                TriggerType.GainHP,
                TriggerTiming.After
            );
        }
    }
}
