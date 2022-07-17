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
    public class AriadneTwistedCardController : CardController
    {
        public AriadneTwistedCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // "Villain cards cannot be damaged or destroyed by Environment cards."
            AddPreventDamageTrigger(
                dda => dda.DamageSource.Is().Environment() &&
                    dda.Target.Is().Villain().AccordingTo(this)
            );

            AddTrigger<DestroyCardAction>(
                dca => 
                    (dca.ResponsibleCard != null && dca.ResponsibleCard.Is().Environment()) ||
                    (dca.ResponsibleCard == null && dca.CardSource?.Card != null && dca.CardSource.Card.Is().Environment()) &&
                    dca.CardToDestroy.Is().Villain().AccordingTo(this),
                dca => CancelAction(dca),
                TriggerType.CancelAction,
                TriggerTiming.Before
            );

            // At the end of the villain turn put the top card of the Environment deck into play
            AddEndOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => GameController.PlayTopCard(
                    DecisionMaker,
                    FindEnvironment(),
                    isPutIntoPlay: true,
                    showMessage: true,
                    cardSource: GetCardSource()),
                TriggerType.PutIntoPlay
            );
        }
    }
}
