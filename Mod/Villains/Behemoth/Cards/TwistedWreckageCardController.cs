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
    public class TwistedWreckageCardController : BehemothUtilityCardController
    {
        public TwistedWreckageCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            // "Change {BehemothCharacter}'s damage type to lightning."
            IEnumerator lightningCoroutine = SetBehemothDamageType(base.Card, DamageType.Lightning);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(lightningCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(lightningCoroutine);
            }
            // "Destroy 2 non-character hero cards."
            List<DestroyCardAction> destroyResults = new List<DestroyCardAction>();
            IEnumerator destroyCoroutine = base.GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.IsHero && !c.IsCharacter && !base.GameController.IsCardIndestructible(c), "hero non-character"), 2, optional: false, requiredDecisions: 2, storedResultsAction: destroyResults, responsibleCard: base.Card, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(destroyCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(destroyCoroutine);
            }
            if (DidDestroyCard(destroyResults))
            {
                // "Remove 1 of them from the game."
                IEnumerable<Card> destroyedCards = (from DestroyCardAction dca in destroyResults where DidDestroyCard(dca) && dca.CardToDestroy.Card != null select dca.CardToDestroy.Card);
                IEnumerable<TurnTaker> destroyedPlayers = (from Card c in destroyedCards select c.Owner).Distinct();
                List<SelectCardDecision> removeChoice = new List<SelectCardDecision>();
                IEnumerator selectCoroutine = base.GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.RemoveCardFromGame, new LinqCardCriteria((Card c) => destroyedCards.Contains(c), "destroyed by " + base.Card.Title, useCardsSuffix: false, useCardsPrefix: true), removeChoice, false, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(selectCoroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(selectCoroutine);
                }
                if (DidSelectCard(removeChoice))
                {
                    Card toRemove = removeChoice.FirstOrDefault().SelectedCard;
                    IEnumerator removeCoroutine = base.GameController.MoveCard(base.TurnTakerController, toRemove, toRemove.Owner.OutOfGame, showMessage: true, responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(removeCoroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(removeCoroutine);
                    }
                }
                // "Move 1 proximity token from another hero to a hero whose card was destroyed this way."
                TurnTaker receivingTT = null;
                if (destroyedPlayers.Count() == 1)
                {
                    receivingTT = destroyedPlayers.FirstOrDefault();
                }
                else
                {
                    List<SelectTurnTakerDecision> receivingChoice = new List<SelectTurnTakerDecision>();
                    IEnumerator selectPlayerCoroutine = base.GameController.SelectTurnTaker(DecisionMaker, SelectionType.AddTokens, receivingChoice, additionalCriteria: (TurnTaker tt) => destroyedPlayers.Contains(tt), numberOfCards: 1, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(selectPlayerCoroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(selectPlayerCoroutine);
                    }
                    if (DidSelectTurnTaker(receivingChoice))
                    {
                        receivingTT = receivingChoice.FirstOrDefault().SelectedTurnTaker;
                    }
                }
                if (receivingTT != null)
                {
                    IEnumerator takeCoroutine = TakeProximityTokens(receivingTT, 1);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(takeCoroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(takeCoroutine);
                    }
                }
            }
            yield break;
        }
    }
}
