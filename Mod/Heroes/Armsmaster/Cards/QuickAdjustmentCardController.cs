using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Armsmaster
{
    public class QuickAdjustmentCardController : CardController
    {
        public QuickAdjustmentCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // At the end of your turn you may return a Module in play to your hand. If you do, you may play a Module
            AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => DoEndOfTurn(pca), TriggerType.PlayCard);

            // Whenever an Equipment card would be destroyed you may destroy this card instead
            AddTrigger<DestroyCardAction>(dca => dca.CardToDestroy.Card.DoKeywordsContain("equipment"), dca => MaybePreventDestruction(dca), TriggerType.CancelAction, TriggerTiming.Before, isActionOptional: true);
        }

        public IEnumerator DoEndOfTurn(PhaseChangeAction pca)
        {
            var storedCard = new List<SelectCardDecision>();
            var e = GameController.SelectCardAndStoreResults(
                HeroTurnTakerController,
                SelectionType.ReturnToHand,
                new LinqCardCriteria(c => c.IsInPlayAndHasGameText && c.DoKeywordsContain("module"), "Module"),
                storedCard,
                optional: true,
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

            var selectedCard = GetSelectedCard(storedCard);
            if (selectedCard == null) { yield break; }

            var storedMove = new List<MoveCardAction>();

            e = GameController.MoveCard(
                TurnTakerController,
                selectedCard,
                HeroTurnTaker.Hand,
                responsibleTurnTaker: TurnTaker,
                storedResults: storedMove,
                actionSource: pca,
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

            var moveCardResult = storedCard.FirstOrDefault();
            if (moveCardResult == null) { yield break; }
            if (moveCardResult.WasCancelled) { yield break; }

            e = SelectAndPlayCardFromHand(
                HeroTurnTakerController,
                optional: true,
                cardCriteria: new LinqCardCriteria(c => c.DoKeywordsContain("module"), "Module")
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

        public IEnumerator MaybePreventDestruction(DestroyCardAction dca)
        {
            var yesnoResult = new List<YesNoCardDecision>();
            var e = GameController.MakeYesNoCardDecision(
                HeroTurnTakerController,
                SelectionType.DestroyCard,
                Card,
                storedResults: yesnoResult,
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

            var result = yesnoResult.FirstOrDefault();
            if (result == null) { yield break; }

            if (result.Answer ?? false)
            {
                e = CancelAction(dca);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }

                e = GameController.DestroyCard(
                    HeroTurnTakerController,
                    Card,
                    responsibleCard: Card,
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
        }
    }
}
