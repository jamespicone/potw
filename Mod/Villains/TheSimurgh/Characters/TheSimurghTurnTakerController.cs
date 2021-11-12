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
    class TheSimurghTurnTakerController : TurnTakerController
    {
        public TheSimurghTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {}

        public override IEnumerator StartGame()
        {
            var traps = FindCardsWhere(c => c.Owner == TurnTaker && c.DoKeywordsContain("trap"));

            var e = GameController.ShuffleCardsIntoLocation(null, traps, TurnTaker.Revealed);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            for (int i = 0; i < 4; ++i)
            {
                var card = TurnTaker.Revealed.TopCard;
                var cardController = FindCardController(card);

                e = GameController.FlipCard(cardController, allowBackToFront: false);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }

                e = GameController.MoveCard(
                    this,
                    card,
                    TurnTaker.PlayArea,
                    playCardIfMovingToPlayArea: false
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

            e = GameController.BulkMoveCards(
                this,
                TurnTaker.Revealed.Cards,
                TurnTaker.OffToTheSide
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var countermeasures = FindCardsWhere(c => c.Owner == TurnTaker && c.Identifier == "ThinkerCountermeasures");
            for (int i = 0; i < (5 - GameController.Game.H); ++i)
            {
                if (countermeasures.Count() <= 0)
                {
                    break;
                }

                e = GameController.PlayCard(
                    this,
                    countermeasures.First(),
                    isPutIntoPlay: true
                );
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }

                countermeasures = countermeasures.Skip(1);
            }

            e = GameController.BulkMoveCards(
                this,
                countermeasures,
                TurnTaker.InTheBox
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
