using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.TheSimurgh
{
    public class ThinkerCountermeasuresCardController : CardController
    {
        public ThinkerCountermeasuresCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            AddTrigger<FlipCardAction>(
                fca => fca.CardToFlip.Card.Is().Villain().AccordingTo(this) && fca.CardToFlip.Card.IsFaceDownNonCharacter,
                fca => DoCountermeasures(fca),
                new TriggerType[] { TriggerType.CancelAction, TriggerType.RemoveFromGame },
                TriggerTiming.Before
            );
        }

        private IEnumerator DoCountermeasures(FlipCardAction fca)
        {
            var e = CancelAction(fca);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            e = GameController.MoveCard(
                TurnTakerController,
                Card,
                TurnTaker.OutOfGame,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }
    }
}
