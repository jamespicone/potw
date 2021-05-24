using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.CoilsBase
{
    public class ConstructionWorkCardController : CardController
    {
        public ConstructionWorkCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "At the end of the environment turn, reveal cards from the top of the environment deck until you reveal three Structure cards. Put them into play and shuffle the other revealed cards back into the environment deck. Then, destroy this card."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, SearchDestructResponse, new TriggerType[] { TriggerType.RevealCard, TriggerType.PutIntoPlay, TriggerType.ShuffleDeck, TriggerType.DestroySelf });
        }

        public IEnumerator SearchDestructResponse(GameAction ga)
        {
            // "... reveal cards from the top of the environment deck until you reveal three Structure cards. Put them into play and shuffle the other revealed cards back into the environment deck."
            IEnumerator revealCoroutine = RevealCards_MoveMatching_ReturnNonMatchingCards(base.TurnTakerController, base.TurnTaker.Deck, false, true, false, new LinqCardCriteria((Card c) => c.DoKeywordsContain("structure"), "structure"), 3, shuffleSourceAfterwards: true, revealedCardDisplay: RevealedCardDisplay.ShowMatchingCards);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(revealCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(revealCoroutine);
            }
            // "Then, destroy this card."
            IEnumerator destroyCoroutine = base.GameController.DestroyCard(DecisionMaker, base.Card, responsibleCard: base.Card, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(destroyCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(destroyCoroutine);
            }
            yield break;
        }
    }
}
