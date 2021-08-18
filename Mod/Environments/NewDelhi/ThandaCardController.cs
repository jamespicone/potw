using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.NewDelhi
{
    public class ThandaCardController : CardController
    {
        private const string PoolIdentifier = "ThandaPool";

        public ThandaCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(RemovedParahumans, () => true).Condition = () => RemovedCount() > 0;
        }

        public string RemovedParahumans()
        {
            string list = (from e in base.Journal.MoveCardEntries() where e.ToLocation.Name == LocationName.OutOfGame && e.CardSource == base.Card select e.Card.Title).ToCommaList(useWordAnd: true);
            return "Parahumans removed from play: " + list + ".";
        }

        public int RemovedCount()
        {
            return (from e in base.Journal.MoveCardEntries() where e.ToLocation.Name == LocationName.OutOfGame && e.CardSource == base.Card select e.Card.Title).Count();
        }

        public override void AddTriggers()
        {
            // "If it ever has no tokens on it, remove it from the game."
            AddTrigger<ModifyTokensAction>((ModifyTokensAction mta) => mta.TokenPool == base.Card.FindTokenPool(PoolIdentifier) && mta.TokenPool.CurrentValue == 0, (ModifyTokensAction mta) => base.GameController.MoveCard(base.TurnTakerController, base.Card, base.TurnTaker.OutOfGame, showMessage: true, responsibleTurnTaker: base.TurnTaker, evenIfIndestructible: true, actionSource: mta, cardSource: GetCardSource()), TriggerType.RemoveFromGame, TriggerTiming.After);
            // "At the end of the environment turn, remove the non-character card Parahuman with the lowest HP from the game. If a card was removed from the game this way, remove a token from this card."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, RemoveParahumanResponse, new TriggerType[] { TriggerType.RemoveFromGame, TriggerType.ModifyTokens });
            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            // "When this card enters play, put three tokens on it."
            IEnumerator resetCoroutine = ResetTokenValue();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(resetCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(resetCoroutine);
            }
        }

        public IEnumerator ResetTokenValue()
        {
            base.Card.FindTokenPool(PoolIdentifier).SetToInitialValue();
            yield return null;
        }

        public IEnumerator RemoveParahumanResponse(PhaseChangeAction pca)
        {
            // "... remove the non-character card Parahuman with the lowest HP from the game."
            List<Card> lowestResults = new List<Card>();
            List<MoveCardAction> moveResults = new List<MoveCardAction>();
            IEnumerator findCoroutine = base.GameController.FindTargetWithLowestHitPoints(1, (Card c) => !c.IsCharacter && c.DoKeywordsContain("parahuman"), lowestResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(findCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(findCoroutine);
            }
            Card lowest = lowestResults.FirstOrDefault();
            if (lowest != null)
            {
                IEnumerator moveCoroutine = base.GameController.MoveCard(base.TurnTakerController, lowest, lowest.Owner.OutOfGame, showMessage: true, responsibleTurnTaker: base.TurnTaker, storedResults: moveResults, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(moveCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(moveCoroutine);
                }
                // "If a card was removed from the game this way, remove a token from this card."
                if (DidMoveCard(moveResults))
                {
                    IEnumerator removeTokenCoroutine = base.GameController.RemoveTokensFromPool(base.Card.FindTokenPool(PoolIdentifier), 1, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(removeTokenCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(removeTokenCoroutine);
                    }
                }
            }
            yield break;
        }
    }
}
