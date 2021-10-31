using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.TheSimurgh
{
    abstract public class ConditionCardController : CardController
    {
        public ConditionCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        protected abstract bool IsConditionMet();

        public override IEnumerator Play()
        {
            if (! IsConditionMet()) { yield break; }

            // flip a face-down villain card and remove this card from the game.
            var e = GameController.SelectAndFlipCards(
                DecisionMaker,
                new LinqCardCriteria(c => c.IsFaceDownNonCharacter && c.Is().Villain().AccordingTo(this), "face-down villain"),
                toFaceDown: false,
                treatAsPutIntoPlay: true,
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

            _trashMe = false;
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

        public override bool DoNotMoveOneShotToTrash => !_trashMe;
        private bool _trashMe = true;
    }
}
