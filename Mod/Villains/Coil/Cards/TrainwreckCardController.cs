using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Coil
{
    public class TrainwreckCardController : CardController
    {
        public TrainwreckCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"Whenever a non-villain target would damage a villain target other than Trainwreck redirect that damage to Trainwreck.",
            AddRedirectDamageTrigger(
                dda => dda.DamageSource.Is(this).NonVillain().Target() && dda.Target.Is(this).Villain().Target() && dda.Target != Card,
                () => Card
            );

            //"At the start of the villain turn Trainwreck regains {H} HP."
            AddEndOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => GameController.GainHP(Card, H, cardSource: GetCardSource()),
                TriggerType.GainHP
            );
        }
    }
}
