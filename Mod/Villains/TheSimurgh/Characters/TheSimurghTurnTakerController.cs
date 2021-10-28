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

            for (int i = 0; i < GameController.Game.H; ++i)
            {
                e = GameController.MoveCard(
                    this,
                    TurnTaker.Revealed.TopCard,
                    TurnTaker.PlayArea,
                    playCardIfMovingToPlayArea: false,
                    flipFaceDown: true
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
        }
    }
}
