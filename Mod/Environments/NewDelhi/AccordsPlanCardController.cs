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
    public class AccordsPlanCardController : NewDelhiOneShotCardController
    {
        public AccordsPlanCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            // "When this card enters play, reveal the top card of each deck in turn order, starting with the villain deck. If the revealed card is an Ongoing, Equipment, or Device, put it into play. Otherwise, shuffle it back into the deck."
            IEnumerator revealCoroutine = DoActionToEachTurnTakerInTurnOrder((TurnTakerController ttc) => true, PlayOrShuffleResponse, responsibleTurnTaker: base.TurnTaker, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(revealCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(revealCoroutine);
            }
            yield break;
        }

        public IEnumerator PlayOrShuffleResponse(TurnTakerController ttc)
        {
            string[] validKeywords = new string[] { "ongoing", "equipment", "device" };
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
                if (revealed.DoKeywordsContain(validKeywords))
                {
                    // "If the revealed card is an Ongoing, Equipment, or Device, put it into play."
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
                    // "Otherwise, shuffle it back into the deck."
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
