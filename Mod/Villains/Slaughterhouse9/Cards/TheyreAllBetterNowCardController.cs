using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Slaughterhouse9
{
    public class TheyreAllBetterNowCardController : CardController
    {
        public TheyreAllBetterNowCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            // Flip the most recently defeated Nine card and set their HP to 10. If no cards are flipped in this way play the top card of the villain deck

            var deadNine = FindCardsWhere(new LinqCardCriteria(
                c => c.DoKeywordsContain("nine") &&
                    c.IsInPlayAndNotUnderCard &&
                    c.IsFlipped
            ));

            var lastDead = Journal.FlipCardEntries().Where(fc => deadNine.Contains(fc.Card) && fc.IsFlipped).LastOrDefault();
            if (lastDead == null || lastDead.Card == null)
            {
                var e = GameController.SendMessageAction("There are no incapacitated Nine to revive!", Priority.High, GetCardSource(), showCardSource: true);
                var e2 = PlayTheTopCardOfTheVillainDeckResponse(null);

                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                    yield return GameController.StartCoroutine(e2);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                    GameController.ExhaustCoroutine(e2);
                }
            }
            else
            {
                var lastDeadController = FindCardController(lastDead.Card);
                var e = GameController.FlipCard(lastDeadController, cardSource: GetCardSource());
                var e2 = GameController.MakeTargettable(lastDead.Card, lastDead.Card.Definition.HitPoints ?? 0, 10, GetCardSource());

                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                    yield return GameController.StartCoroutine(e2);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                    GameController.ExhaustCoroutine(e2);
                }
            }
        }
    }
}
