using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Coil
{
    public class ABackupForEveryAssetCardController : CardController
    {
        public ABackupForEveryAssetCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            // "Take all Ongoing cards out of the villain trash and put them into play"
            // TODO: This should probably *select* a villain trash, although I guess that can't happen in a only-one-villain game
            var cardsToMove = FindCardsWhere(new LinqCardCriteria(c => c.IsOngoing && c.Location.IsVillain && c.Location.IsTrash));
            var e = GameController.MoveCards(TurnTakerController, cardsToMove, TurnTaker.PlayArea, isPutIntoPlay: true, cardSource: GetCardSource());
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
