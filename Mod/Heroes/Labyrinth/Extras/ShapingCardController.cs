using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra;

using Jp.SOTMUtilities;
using Unity.IO.LowLevel.Unsafe;

namespace Jp.ParahumansOfTheWormverse.Labyrinth
{

    public abstract class ShapingCardController : CardController
    {
        public ShapingCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowListOfCards(
                new LinqCardCriteria((Card c) => c.IsUnderCard && c.Location == Card.UnderLocation, "under this card", useCardsSuffix: false, useCardsPrefix: true)
            );
        }

        public abstract void AddShapingTriggers();

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            var possibleBases = FindCardsWhere((Card c) => c.IsInPlayAndNotUnderCard && c.Is().Environment());
            if (possibleBases.Count() <= 0)
            {
                storedResults?.Add(new MoveCardDestination(TurnTaker.Trash));
                yield break;
            }

            var storedLocation = new List<SelectCardDecision>();
            var e = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.MoveCardAboveCard, possibleBases, storedLocation);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            var selected = GetSelectedCard(storedLocation);
            if (selected == null)
            {
                storedResults?.Add(new MoveCardDestination(TurnTaker.Trash));
                yield break;
            }

            var originalLocation = selected.Location;
            e = GameController.BulkMoveCards(
                TurnTakerController,
                new List<Card> { selected },
                Card.UnderLocation,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            Card.PlayIndex = selected.PlayIndex;
            selected.PlayIndex = null;
            storedResults?.Add(new MoveCardDestination(originalLocation));
        }

        public override void AddTriggers()
        {
            AddTrigger<MoveCardAction>((mca) => Card.UnderLocation.IsEmpty, DestroyThisCardResponse, TriggerType.DestroySelf, TriggerTiming.After);
            AddTrigger<BulkMoveCardsAction>((mca) => Card.UnderLocation.IsEmpty, DestroyThisCardResponse, TriggerType.DestroySelf, TriggerTiming.After);
            
            AddBeforeLeavesPlayActions(PutUnderCardsBackInPlay);

            // Could do this with AddBeforeLeavesPlayAction, but that will fire with a null gameaction when
            // putting shapings under other cards. This way we don't have that issue and still get triggered
            // when we're incapped, which doesn't happen with AddBeforeLeavesPlayActions.
            AddTrigger<BulkMoveCardsAction>(
                mca => mca.CardsToMove.Contains(Card) && !mca.Destination.IsInPlay,
                PutUnderCardsBackInPlay,
                TriggerType.MoveCard,
                TriggerTiming.Before
            );

            AddShapingTriggers();
        }

        public override IEnumerator Play()
        {
            if (Card.UnderLocation.IsEmpty)
            {
                var e = DestroyThisCardResponse(null);
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }

            yield break;
        }

        private IEnumerator PutUnderCardsBackInPlay(GameAction action)
        {
            // Cards being moved under cards trigger their before-leaves-play actions;
            // we don't want that.
            {
                if (action is MoveCardAction mca)
                {
                    if (mca.Destination.IsInPlay)
                    {
                        yield break;
                    }
                }
            }

            {
                if (action is BulkMoveCardsAction mca)
                {
                    if (mca.Destination.IsInPlay)
                    {
                        yield break;
                    }
                }
            }

            var undercards = Card.UnderLocation.Cards.ToList();
            foreach (Card card in undercards)
            {
                var e = GameController.MoveCard(TurnTakerController, card, Card.Location, playCardIfMovingToPlayArea: false, doesNotEnterPlay: true, cardSource: GetCardSource());
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }

                card.PlayIndex = Card.PlayIndex;
                Card.PlayIndex = null;
                FindCardController(card).AddAllTriggers();
            }
        }
    }}
