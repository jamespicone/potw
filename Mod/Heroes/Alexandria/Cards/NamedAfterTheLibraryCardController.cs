using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Alexandria
{
    public class NamedAfterTheLibraryCardController : CardController
    {
        public NamedAfterTheLibraryCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // "When this card enters play shuffle your trash into your deck",
            var e = GameController.ShuffleTrashIntoDeck(
                TurnTakerController,
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

        public override void AddTriggers()
        {
            // "Whenever you would draw a card instead reveal the top 2 cards of your deck, put 1 into your hand, and return the other to the top of your deck"
            AddTrigger<DrawCardAction>(
                dca => dca.HeroTurnTaker == HeroTurnTaker && dca.IsSuccessful,
                dca => RevealAndDraw(dca),
                new TriggerType[] { TriggerType.RevealCard, TriggerType.CancelAction },
                TriggerTiming.Before
            );
        }

        private IEnumerator RevealAndDraw(DrawCardAction dca)
        {
            var e = CancelAction(dca, showOutput: false);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var revealedCards = new List<Card>();
            e = GameController.RevealCards(
                TurnTakerController,
                TurnTaker.Deck,
                numberOfCards: 2,
                revealedCards,
                revealedCardDisplay: RevealedCardDisplay.None,
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

            e = GameController.SelectAndMoveCard(
                HeroTurnTakerController,
                c => revealedCards.Contains(c),
                HeroTurnTaker.Hand,
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

            e = CleanupRevealedCards(TurnTaker.Revealed, TurnTaker.Deck);
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
