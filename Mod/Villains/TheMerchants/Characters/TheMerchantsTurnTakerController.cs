using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.TheMerchants
{
    class TheMerchantsTurnTakerController : TurnTakerController
    {
        public TheMerchantsTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public override IEnumerator StartGame()
        {
            // Put all Thug cards under Skidmark and shuffle them
            IEnumerable<Card> thugs = base.GameController.FindCardsWhere(new LinqCardCriteria((Card c) => c.Owner == base.TurnTaker && c.DoKeywordsContain("thug"), "thug"));
            Log.Debug("thugs.Count(): " + thugs.Count().ToString());
            IEnumerator deckCoroutine = base.GameController.BulkMoveCards(this, thugs, base.CharacterCard.UnderLocation, responsibleTurnTaker: base.TurnTaker, cardSource: FindCardController(base.CharacterCard).GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(deckCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(deckCoroutine);
            }
            IEnumerator shuffleCoroutine = base.GameController.ShuffleLocation(base.CharacterCard.UnderLocation, cardSource: FindCardController(base.CharacterCard).GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(shuffleCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(shuffleCoroutine);
            }
            yield break;
        }

        public override void AddTriggers()
        {
            // If a Thug would be moved to the villain deck or trash, shuffle it into the Thug deck instead
            CardSource cardSource = new CardSource(base.CharacterCardController);
            Trigger<MoveCardAction> shuffleThugTrigger = new Trigger<MoveCardAction>(base.GameController, (MoveCardAction mca) => mca.CardToMove.Owner == base.TurnTaker && mca.CardToMove.DoKeywordsContain("thug") && (mca.Destination == base.TurnTaker.Deck || mca.Destination == base.TurnTaker.Trash), ShuffleThugResponse, new TriggerType[] { TriggerType.CancelAction, TriggerType.MoveCard }, TriggerTiming.Before, cardSource);
            Trigger<BulkMoveCardsAction> shuffleThugsTrigger = new Trigger<BulkMoveCardsAction>(base.GameController, (BulkMoveCardsAction bmca) => bmca.CardsToMove.Any((Card c) => c.Owner == base.TurnTaker && c.DoKeywordsContain("thug")) && (bmca.Destination == base.TurnTaker.Deck || bmca.Destination == base.TurnTaker.Trash), ShuffleThugsResponse, new TriggerType[] { TriggerType.CancelAction, TriggerType.MoveCard }, TriggerTiming.Before, cardSource);
            base.CharacterCardController.AddTrigger(shuffleThugTrigger);
            base.CharacterCardController.AddTrigger(shuffleThugsTrigger);
            base.AddTriggers();
        }

        private IEnumerator ShuffleThugResponse(MoveCardAction mca)
        {
            // If a Thug would be moved to the villain deck or trash, shuffle it into the Thug deck instead
            CardSource cardSource = new CardSource(base.CharacterCardController);
            IEnumerator cancelCoroutine = base.GameController.CancelAction(mca, cardSource: cardSource);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(cancelCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(cancelCoroutine);
            }
            IEnumerator shuffleCoroutine = base.GameController.ShuffleCardIntoLocation(base.FindDecisionMaker(), mca.CardToMove, base.CharacterCard.UnderLocation, false, cardSource: cardSource);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(shuffleCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(shuffleCoroutine);
            }
            yield break;
        }

        private IEnumerator ShuffleThugsResponse(BulkMoveCardsAction bmca)
        {
            // If a Thug would be moved to the villain deck or trash, shuffle it into the Thug deck instead
            CardSource cardSource = new CardSource(base.CharacterCardController);
            List<Card> thugs = bmca.CardsToMove.Where((Card c) => c.DoKeywordsContain("thug")).ToList();
            if (bmca.CardsToMove.Any((Card c) => !thugs.Contains(c)))
            {
                bmca.RemoveCardsFromMove(thugs);
            }
            else
            {
                IEnumerator cancelCoroutine = base.GameController.CancelAction(bmca, cardSource: cardSource);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(cancelCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(cancelCoroutine);
                }
            }
            IEnumerator shuffleCoroutine = base.GameController.ShuffleCardsIntoLocation(FindDecisionMaker(), thugs, base.CharacterCard.UnderLocation, cardSource: cardSource);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(shuffleCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(shuffleCoroutine);
            }
            // ...
            yield break;
        }
    }
}
