using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Labyrinth
{
    public class ExplorationCardController : CardController
    {
        public ExplorationCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // "You may draw a card.",
            var e = DrawCard(optional: true);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // "Put the top card of the environment deck under Labyrinth.",
            var card = FindEnvironment().TurnTaker.Deck.TopCard;
            if (card != null)
            {
                e = GameController.MoveCard(
                    TurnTakerController,
                    card,
                    CharacterCard.UnderLocation,
                    cardSource: GetCardSource()
                );
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }

            // "Put the top card of the environment deck into play."
            e = GameController.PlayTopCard(
                HeroTurnTakerController,
                FindEnvironment(),
                numberOfCards: 1,
                isPutIntoPlay: true,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
