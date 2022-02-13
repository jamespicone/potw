using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;
using UnityEngine;

namespace Jp.ParahumansOfTheWormverse.Grue
{
    public class DarknessCardController : CardController
    {
        public DarknessCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // "This card doesn't reduce damage dealt by {GrueCharacter}",
            // "Reduce the first damage that would be dealt each turn by the card next to this card by 1",
            _reduceSourceTrigger = AddReduceDamageTrigger(
                dda => ! HasBeenSetToTrueThisTurn("FirstDamageDealtSource") && ! dda.DamageSource.IsGrueSource(CharacterCard) && Card.Location.IsNextToCard && dda.DamageSource.Card == GetCardThisCardIsNextTo(),
                dda => ReduceFirstEachTurnByOneSource(dda),
                dda => true,
                true
            );

            // "Reduce the first damage that would be dealt each turn to the card next to this card by 1",
            _reduceTargetTrigger = AddReduceDamageTrigger(
                dda => ! HasBeenSetToTrueThisTurn("FirstDamageDealtTarget") && ! dda.DamageSource.IsGrueSource(CharacterCard) && Card.Location.IsNextToCard && dda.Target == GetCardThisCardIsNextTo(),
                dda => ReduceFirstEachTurnByOneTarget(dda),
                dda => true,
                true
            );

            // "At the end of {GrueCharacter}'s next turn remove this card from the game"
            AddEndOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => RemoveSelfFromGame(pca),
                TriggerType.RemoveFromGame,
                pca => IsGruesNextTurn(pca)
            );

            AddIfTheTargetThatThisCardIsNextToLeavesPlayDestroyThisCardTrigger();

            AddTrigger<MoveCardAction>(
                mca => mca.CardToMove == Card && !mca.Destination.IsOutOfGame && !mca.Destination.IsInPlay,
                mca => GoOutOfPlay(mca),
                TriggerType.RemoveFromGame,
                TriggerTiming.Before
            );

            AddTrigger<DestroyCardAction>(
                dca => dca.CardToDestroy == this && dca.IsSuccessful,
                dca => GoOutOfPlay(dca),
                TriggerType.ChangePostDestroyDestination,
                TriggerTiming.Before
            );
        }

        private IEnumerator ReduceFirstEachTurnByOneSource(DealDamageAction dda)
        {
            SetCardPropertyToTrueIfRealAction("FirstDamageDealtSource");
            return GameController.ReduceDamage(dda, 1, _reduceSourceTrigger, GetCardSource());
        }

        private IEnumerator ReduceFirstEachTurnByOneTarget(DealDamageAction dda)
        {
            SetCardPropertyToTrueIfRealAction("FirstDamageDealtTarget");
            return GameController.ReduceDamage(dda, 1, _reduceTargetTrigger, GetCardSource());
        }

        private bool IsGruesNextTurn(PhaseChangeAction pca)
        {
            // If this card entered play this turn it's not Grue's *next* turn yet.
            if (Journal.CardEntersPlayEntriesThisTurn().Where(je => je.Card == Card).Count() > 0)
            {
                return false;
            }

            if (pca.ToPhase.TurnTaker != TurnTaker || pca.ToPhase.Phase != Phase.End)
            {
                return false;
            }

            return true;
        }

        private IEnumerator RemoveSelfFromGame(PhaseChangeAction pca)
        {
            return GameController.MoveCard(
                TurnTakerController,
                Card,
                TurnTaker.OutOfGame,
                cardSource: GetCardSource()
            );
        }

        private IEnumerator GoOutOfPlay(MoveCardAction mca)
        {
            mca.SetDestination(TurnTaker.OutOfGame, destinationCanBeChanged: true);
            yield break;
        }

        private IEnumerator GoOutOfPlay(DestroyCardAction dca)
        {
            dca.OverridePostDestroyDestination(TurnTaker.OutOfGame, cardSource: GetCardSource());
            yield break;
        }

        private ITrigger _reduceSourceTrigger;
        private ITrigger _reduceTargetTrigger;
    }
}
