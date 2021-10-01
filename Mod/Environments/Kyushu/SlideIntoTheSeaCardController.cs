using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Kyushu
{
    public class SlideIntoTheSeaCardController : CardController
    {
        public SlideIntoTheSeaCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(OtherEnvironmentCards());
            SpecialStringMaker.ShowNumberOfCardsAtLocations(() => new Location[2]{ base.TurnTaker.Deck, base.TurnTaker.Trash }, new LinqCardCriteria((Card c) => true), () => base.Card.IsInPlayAndHasGameText);
        }

        public LinqCardCriteria OtherEnvironmentCards()
        {
            return new LinqCardCriteria((Card c) => c.Alignment().Environment() && c != base.Card && c.IsInPlay && !c.IsBeingDestroyed && base.GameController.IsCardVisibleToCardSource(c, GetCardSource()), "environment cards other than " + base.Card.Title, false, false, "environment card other than " + base.Card.Title, "environment cards other than " + base.Card.Title);
        }

        public override void AddTriggers()
        {
            // "At the end of the environment turn, play the top card of the environment deck, shuffle the top 2 cards of the environment trash into the environment deck, then remove the top card of the environment deck from the game."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, PlayShuffleRemoveResponse, new TriggerType[] { TriggerType.PlayCard, TriggerType.ShuffleCardIntoDeck, TriggerType.RemoveFromGame });
            // "At the start of the environment turn, if there are no cards in the environment deck, the island sinks. [b]GAME OVER.[/b]"
            AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DeckCheckResponse, TriggerType.GameOver);
            // "If this card would be destroyed while another environment card is in play, destroy that card instead."
            base.AddTrigger<DestroyCardAction>((DestroyCardAction dca) => dca.CardToDestroy.Card == base.Card && FindCardsWhere(OtherEnvironmentCards()).Count() > 0, DestroyOtherCardInsteadResponse, new TriggerType[] { TriggerType.CancelAction, TriggerType.DestroyCard }, TriggerTiming.Before);
            base.AddTriggers();
        }

        public IEnumerator PlayShuffleRemoveResponse(PhaseChangeAction pca)
        {
            // "... play the top card of the environment deck..."
            IEnumerator playCoroutine = base.GameController.PlayTopCard(DecisionMaker, base.TurnTakerController, responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(playCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(playCoroutine);
            }
            // "... shuffle the top 2 cards of the environment trash into the environment deck..."
            IEnumerator shuffleCoroutine = base.GameController.ShuffleCardsIntoLocation(DecisionMaker, base.TurnTaker.Trash.GetTopCards(2), base.TurnTaker.Deck, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(shuffleCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(shuffleCoroutine);
            }
            // "... then remove the top card of the environment deck from the game."
            IEnumerator removeCoroutine = base.GameController.MoveCard(base.TurnTakerController, base.TurnTaker.Deck.TopCard, base.TurnTaker.OutOfGame, responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(removeCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(removeCoroutine);
            }
            yield break;
        }

        public IEnumerator DeckCheckResponse(PhaseChangeAction pca)
        {
            // "... if there are no cards in the environment deck..."
            if (!base.TurnTaker.Deck.HasCards)
            {
                // "... the island sinks. [b]GAME OVER.[/b]"
                IEnumerator sinkCoroutine = base.GameController.GameOver(EndingResult.EnvironmentDefeat, "The damage to the island is too great! Kyushu is submerged!", true, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(sinkCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(sinkCoroutine);
                }
            }
            yield break;
        }

        public IEnumerator DestroyOtherCardInsteadResponse(DestroyCardAction dca)
        {
            // "If this card would be destroyed while another environment card is in play, destroy that card instead."
            IEnumerable<Card> otherCards = FindCardsWhere(OtherEnvironmentCards(), GetCardSource());
            if (otherCards != null && otherCards.Count() > 0)
            {
                IEnumerator preventCoroutine = base.GameController.CancelAction(dca, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(preventCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(preventCoroutine);
                }

                IEnumerator destroyCoroutine = base.GameController.SelectAndDestroyCard(DecisionMaker, OtherEnvironmentCards(), false, responsibleCard: base.Card, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(destroyCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(destroyCoroutine);
                }
            }
            yield break;
        }
    }
}
