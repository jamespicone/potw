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
    public class SomeMoreFriendsCardController : CardController
    {
        public SomeMoreFriendsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            // Move the top card under the Slaughterhouse 9 card into the villain play area.
            var nineCard = FindCard("Slaughterhouse9Character", realCardsOnly: false);
            if (nineCard == null) {
                var e = GameController.SendMessageAction(
                    "Couldn't find slaughterhouse9 character",
                    Priority.Low,
                    GetCardSource()
                );
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }
                
                yield break;
            }

            var cardToMove = nineCard.UnderLocation.TopCard;
            if (cardToMove == null)
            {
                var e = GameController.SendMessageAction(
                    "There were no cards under the Slaughterhouse 9 card to put into play",
                    Priority.Low,
                    GetCardSource()
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
            else
            {
                var e = GameController.PlayCard(
                    TurnTakerController,
                    cardToMove,
                    isPutIntoPlay: true,
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

            // Play the top card of the villain deck
            var e2 = PlayTheTopCardOfTheVillainDeckResponse(null);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e2);
            }
            else
            {
                GameController.ExhaustCoroutine(e2);
            }
        }
    }
}
