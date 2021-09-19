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
    public class ParahumanPrisonCardController : CardController
    {
        public ParahumanPrisonCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsUnderCard(card);

            AddThisCardControllerToList(CardControllerListType.ChangesVisibility);
        }

        public override void AddTriggers()
        {
            // "Whenever a target is destroyed put it under this card face-down, then put any other cards under this card into play face-up",
            AddTrigger<DestroyCardAction>(
                dca => dca.CardToDestroy.Card.IsTarget && dca.CardToDestroy.Card != Card && ! dca.CardToDestroy.Card.DoKeywordsContain("structure"),
                dca => JailTarget(dca),
                TriggerType.ChangePostDestroyDestination,
                TriggerTiming.Before
            );

            AddTrigger<MoveCardAction>(
                mca => mca.Destination == Card.UnderLocation,
                mca => FreeCardsUnderThisCard(mca.CardToMove),
                TriggerType.PutIntoPlay,
                TriggerTiming.After
            );

            // "When this card is destroyed put any cards under it into play face-up."
            AddBeforeLeavesPlayAction(ga => FreeCardsUnderThisCard(), TriggerType.PutIntoPlay);
            AddBeforeLeavesPlayActions(ga => FreeCardsUnderThisCard());
        }

        public override bool? AskIfCardIsVisibleToCardSource(Card card, CardSource cardSource)
        {
            if (card.Location == Card.UnderLocation && cardSource.Card != Card) { return false; }
            return null;
        }

        private IEnumerator FreeCardsUnderThisCard(Card except = null)
        {
            var cardsToPlay = Card.UnderLocation.Cards.Where(c => c != except);
            var e = GameController.PlayCards(
                DecisionMaker,
                c => cardsToPlay.Contains(c),
                optional: false,
                isPutIntoPlay: true,
                allowAutoDecide: true,
                cardTypeDescription: $"cards under {Card.Title}",
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

        private IEnumerator JailTarget(DestroyCardAction dca)
        {
            if (! dca.PostDestroyDestinationCanBeChanged) { yield break; }

            dca.SetPostDestroyDestination(
                Card.UnderLocation,
                showMessage: true,
                cardSource: GetCardSource()
            );
        }
    }
}
