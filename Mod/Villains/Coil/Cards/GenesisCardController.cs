using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

using UnityEngine;

namespace Jp.ParahumansOfTheWormverse.Coil
{
    public class GenesisCardController : CardController
    {
        public GenesisCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // "When this card is destroyed shuffle it back into the villain deck.",
            // This needs to trigger out-of-play because the card will be inhibited by the time it triggers.
            // We don't set the card source on the post-destroy move (because of the inhibitor), but we do pass the
            // decision sources, so we can use them to smuggle the info that it's the move from this card.
            AddTrigger<MoveCardAction>(
                mca => mca.CardToMove == Card &&
                    mca.Destination.IsDeck &&
                    mca.Destination.IsVillain &&
                    (mca.DecisionSources?.Count(d => d.CardSource?.Card == Card) ?? 0) > 0 &&
                    mca.CardSource == null &&
                    mca.Origin.IsPlayArea &&
                    mca.ActionSource == _actionThatDestroyedMe,
                mca => EnsureShuffle(mca),
                TriggerType.ShuffleDeck,
                TriggerTiming.Before,
                outOfPlayTrigger: true
            );

            AddWhenDestroyedTrigger(
                SetPostDestroyLocation,
                new TriggerType[2]
                {
                    TriggerType.MoveCard,
                    TriggerType.ChangePostDestroyDestination
                }, 
                dca => dca.PostDestroyDestinationCanBeChanged
            );

            //"At the end of the villain turn this card deals 2 poison damage to {H} hero targets."
            AddDealDamageAtEndOfTurnTrigger(
                TurnTaker,
                Card,
                c => c.Is(this).Hero().Target(),
                TargetType.SelectTarget,
                2,
                DamageType.Toxic,
                numberOfTargets: H
            );
        }

        private IEnumerator EnsureShuffle(MoveCardAction mca)
        {
            mca.ShuffleDestinationAfterMove = true;
            yield break;
        }

        private IEnumerator SetPostDestroyLocation(DestroyCardAction dca)
        {
            if (! dca.PostDestroyDestinationCanBeChanged)
            {
                yield break;
            }

            AddInhibitorException((ga) => ga is MoveCardAction || ga is MakeDecisionAction);

            var storedResults = new List<SelectLocationDecision>();
            var e = FindVillainDeck(
                DecisionMaker,
                SelectionType.ShuffleCardIntoDeck,
                storedResults,
                l => l.IsVillain
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var villainDeck = GetSelectedLocation(storedResults);
            if (villainDeck == null) { yield break; }

            // Don't set card source because actions with a card source of this card are about to get inhibited.
            // No clear point to undo an inhibitor exception.
            // This is how Crystalloid Behemoth is implemented in the base game.
            dca.SetPostDestroyDestination(villainDeck, decisionSources: storedResults.Cast<IDecision>());
            _actionThatDestroyedMe = dca;

            RemoveInhibitorException();
        }

        private DestroyCardAction _actionThatDestroyedMe;
    }
}
