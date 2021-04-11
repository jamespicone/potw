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
    public class AttentionCardController : CardController
    {
        public AttentionCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "At the end of the environment turn, reveal the top card of each deck in turn order, starting with the villain deck. If the revealed card is a target, put it into play. Otherwise, shuffle it back into its deck. Then, destroy this card."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, RevealDestroySequence, new TriggerType[] { TriggerType.RevealCard, TriggerType.PutIntoPlay, TriggerType.ShuffleCardIntoDeck, TriggerType.DestroySelf });
            base.AddTriggers();
        }

        public IEnumerator RevealDestroySequence(PhaseChangeAction pca)
        {
            // "... reveal the top card of each deck in turn order, starting with the villain deck. If the revealed card is a target, put it into play. Otherwise, shuffle it back into its deck."
            IEnumerator revealCoroutine = DoActionToEachTurnTakerInTurnOrder((TurnTakerController ttc) => true, PlayOrShuffleResponse, responsibleTurnTaker: base.TurnTaker, true);
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

        public IEnumerator PlayOrShuffleResponse(TurnTakerController ttc)
        {
            // Reveal the top card of [TurnTaker]'s deck
            List<Card> revealResults = new List<Card>();
            IEnumerator revealCoroutine = base.GameController.RevealCards(base.TurnTakerController, ttc.TurnTaker.Deck, 1, revealResults, revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(revealCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(revealCoroutine);
            }
            Card revealed = revealResults.FirstOrDefault();
            if (revealed != null)
            {
                if (revealed.IsTarget)
                {
                    // "If the revealed card is a target, put it into play."
                    IEnumerator playCoroutine = base.GameController.PlayCard(ttc, revealed, isPutIntoPlay: true, responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(playCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(playCoroutine);
                    }
                }
                else
                {
                    // "Otherwise, shuffle it back into its deck."
                    IEnumerator shuffleCoroutine = base.GameController.ShuffleCardIntoLocation(DecisionMaker, revealed, ttc.TurnTaker.Deck, false, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(shuffleCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(shuffleCoroutine);
                    }
                }
            }
            yield break;
        }
    }
}
