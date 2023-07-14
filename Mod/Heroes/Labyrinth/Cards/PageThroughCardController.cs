using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;
using UnityEngine;

namespace Jp.ParahumansOfTheWormverse.Labyrinth
{
    public class PageThroughCardController : CardController
    {
        public PageThroughCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => MoveCardUnderLabyrinth(), TriggerType.MoveCard);
        }

        private IEnumerator MoveCardUnderLabyrinth()
        {
            var card = FindEnvironment().TurnTaker.Deck.TopCard;
            if (card == null) { yield break; }

            var e = GameController.MoveCard(
                TurnTakerController,
                card,
                CharacterCard.UnderLocation,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // Draw 2 cards.
            var e = DrawCards(HeroTurnTakerController, 2);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            var envdeck = FindEnvironment().TurnTaker.Deck;

            // Reveal the top 2 cards of the environment deck.
            var revealed = new List<Card>();
            e = GameController.RevealCards(
                TurnTakerController,
                envdeck,
                GetPowerNumeral(0, 2),
                revealed,
                revealedCardDisplay: RevealedCardDisplay.None,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // Discard up to 1 card;
            e = GameController.SelectCardsAndDoAction(
                HeroTurnTakerController,
                new LinqCardCriteria(c => revealed.Contains(c) && c.Location == envdeck.OwnerTurnTaker.Revealed, "revealed"),
                SelectionType.DiscardCard,
                c => GameController.DiscardCard(HeroTurnTakerController, c, null, cardSource: GetCardSource()),
                numberOfCards: GetPowerNumeral(1, 1),
                requiredDecisions: 0,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            //  put the rest into play in any order."
            e = GameController.PlayCards(
                HeroTurnTakerController,
                c => revealed.Contains(c) && c.Location == envdeck.OwnerTurnTaker.Revealed,
                optional: false,
                isPutIntoPlay: true,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            e = CleanupCardsAtLocations(
                new List<Location> { envdeck },
                envdeck,
                shuffleAfterwards: true,
                cardsInList: revealed
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
