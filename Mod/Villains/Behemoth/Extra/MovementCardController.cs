using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Behemoth
{
    public class MovementCardController : BehemothUtilityCardController
    {
        public const string MovementTrashIdentifier = "MovementTrash";

        public MovementCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            // Show all Proximity pools?
        }

        public override IEnumerator Play()
        {
            // After card is played, move it under MovementTrash
            Log.Debug("MovementCardController.Play() activated");
            IEnumerator trashCoroutine = base.GameController.MoveCard(base.TurnTakerController, base.Card, base.TurnTaker.FindCard(MovementTrashIdentifier, realCardsOnly: false).UnderLocation, playCardIfMovingToPlayArea: false, responsibleTurnTaker: base.TurnTaker, doesNotEnterPlay: true, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(trashCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(trashCoroutine);
            }
            yield break;
        }
    }
}
