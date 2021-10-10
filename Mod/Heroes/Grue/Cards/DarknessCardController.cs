using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.ParahumansOfTheWormverse.Utility;
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
                dda => ! IsPropertyTrue("FirstDamageDealtSource") && ! dda.DamageSource.IsGrueSource(CharacterCard) && Card.Location.IsNextToCard && dda.DamageSource.Card == GetCardThisCardIsNextTo(),
                dda => ReduceFirstEachTurnByOneSource(dda),
                dda => true,
                true
            );

            // "Reduce the first damage that would be dealt each turn to the card next to this card by 1",
            _reduceTargetTrigger = AddReduceDamageTrigger(
                dda => !IsPropertyTrue("FirstDamageDealtTarget") && !dda.DamageSource.IsGrueSource(CharacterCard) && Card.Location.IsNextToCard && dda.Target == GetCardThisCardIsNextTo(),
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
            Debug.Log($"Checking if Grue's next turn is [{pca}]");

            // If this card entered play this turn it's not Grue's *next* turn yet.
            if (Journal.CardEntersPlayEntriesThisTurn().Where(je => je.Card == Card).Count() > 0)
            {
                Debug.Log("Card entered play this turn");
                return false;
            }

            if (pca.ToPhase.TurnTaker != TurnTaker || pca.ToPhase.Phase != Phase.End)
            {
                Debug.Log("Not Grue's turn or not end phase");
                return false;
            }

            return true;
        }

        private IEnumerator RemoveSelfFromGame(PhaseChangeAction pca)
        {
            Debug.Log("Removing darkness from game");
            return GameController.MoveCard(
                TurnTakerController,
                Card,
                TurnTaker.OutOfGame,
                cardSource: GetCardSource()
            );
        }

        private ITrigger _reduceSourceTrigger;
        private ITrigger _reduceTargetTrigger;
    }
}
