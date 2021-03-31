using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Tattletale
{
    public class OutOfCommissionCardController : CardController
    {
        public OutOfCommissionCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            // "{TattletaleCharacter} regains 5 HP."
            IEnumerator healCoroutine = base.GameController.GainHP(base.CharacterCard, 5, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(healCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(healCoroutine);
            }
            // "Immediately end your turn."
            IEnumerator endCoroutine = base.GameController.ImmediatelyEndTurn(base.TurnTakerController, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(endCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(endCoroutine);
            }
            yield break;
        }
    }
}
