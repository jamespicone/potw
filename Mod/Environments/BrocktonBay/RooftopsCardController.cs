using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.BrocktonBay
{
    public class RooftopsCardController : CardController
    {
        public RooftopsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "Increase all damage by 1."
            AddIncreaseDamageTrigger((DealDamageAction dda) => true, 1);
            // "At the end of their turn, a player may discard 2 cards to destroy this card."
            AddEndOfTurnTrigger((TurnTaker tt) => tt.IsHero, DiscardToDestroyResponse, new TriggerType[] { TriggerType.DiscardCard, TriggerType.DestroySelf }, (PhaseChangeAction pca) => pca.ToPhase.TurnTaker.ToHero().Hand.Cards.Count() >= 2);
            base.AddTriggers();
        }

        public IEnumerator DiscardToDestroyResponse(PhaseChangeAction pca)
        {
            // "... [player] may discard 2 cards to destroy this card."
            List<DiscardCardAction> discardResults = new List<DiscardCardAction>();
            if (pca.ToPhase.TurnTaker.IsHero)
            {
                IEnumerator discardCoroutine = SelectAndDiscardCards(base.GameController.FindHeroTurnTakerController(pca.ToPhase.TurnTaker.ToHero()), 2, optional: true, storedResults: discardResults, responsibleTurnTaker: base.TurnTaker);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(discardCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(discardCoroutine);
                }
                if (DidDiscardCards(discardResults, numberExpected: 2))
                {
                    IEnumerator destroyCoroutine = base.GameController.DestroyCard(DecisionMaker, base.Card, responsibleCard: base.Card, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(destroyCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(destroyCoroutine);
                    }
                }
            }
            yield break;
        }
    }
}
