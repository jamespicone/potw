﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Jp.ParahumansOfTheWormverse.Dragon
{
    public class DragonCharacterCardController : HeroCharacterCardController, IFocusPoolController
    {
        public DragonCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            AddThisCardControllerToList(CardControllerListType.ChangesPhaseOrder);
        }

        public override void AddSideTriggers()
        {
            if(! Card.IsFlipped)
            {
                AddSideTrigger(AddPhaseChangeTrigger(tt => tt == TurnTaker, p => p == Phase.Start, pca => true,
                    pca => AddFocusTokens(4, GetCardSource()), new TriggerType[] { TriggerType.FirstTrigger }, TriggerTiming.Before));

                AddSideTrigger(AddPhaseChangeTrigger(tt => tt == TurnTaker, p => p == Phase.End, pca => true,
                    pca => ResetFocus(), new TriggerType[] { TriggerType.FirstTrigger }, TriggerTiming.Before));

                AddSideTrigger(AddPhaseChangeTrigger(tt => tt == TurnTaker, p => p == Phase.Unknown, 
                    pca => true, pca => DoFocusActions(), new TriggerType[] { TriggerType.UsePower }, TriggerTiming.Before ));
            }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator e;

            switch(index)
            {
                default: yield break;
                case 0:
                    // One target regains 2 HP
                    e = GameController.SelectAndGainHP(HeroTurnTakerController, 2, cardSource: GetCardSource());
                    break;

                case 1:
                    // One player may take a card from their trash into their hand.
                    e = GameController.SelectHeroToMoveCardFromTrash(
                        HeroTurnTakerController,
                        httc => httc.HeroTurnTaker.Hand,
                        cardSource: GetCardSource()
                    );
                    break;

                case 2:
                    // Reduce damage dealt by environment targets by 2 until the start of your next turn.
                    var status = new ReduceDamageStatusEffect(2);
                    status.SourceCriteria.IsEnvironment = true;
                    status.SourceCriteria.IsTarget = true;
                    status.UntilStartOfNextTurn(TurnTaker);

                    e = GameController.AddStatusEffect(status, true, GetCardSource());
                    break;
            }

            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // Activate a Focus effect
            var e = GameController.SelectAndActivateAbility(
                HeroTurnTakerController,
                "focus",
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

        private IEnumerator DoFocusActions()
        {
            var pool = CharacterCard.FindTokenPool("FocusPool");
            if (pool == null) { yield break; }

            while (pool.CurrentValue > 0)
            {
                var selectedAbility = new List<ActivateAbilityDecision>();
                var e = GameController.SelectAndActivateAbility(
                    HeroTurnTakerController,
                    "focus",
                    storedResults: selectedAbility,
                    optional: true,
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

                if (selectedAbility.Count() <= 0 || selectedAbility.First().Skipped) { break; }

                e = GameController.RemoveTokensFromPool(pool, 1, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }
            }
        }

        public IEnumerator AddFocusTokens(int tokens, CardSource source)
        {
            var pool = CharacterCard.FindTokenPool("FocusPool");
            if (pool == null) { yield break; }

            var e = GameController.AddTokensToPool(pool, tokens, source);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        public IEnumerator LoseFocusTokens(int tokens, CardSource source)
        {
            var pool = CharacterCard.FindTokenPool("FocusPool");
            if (pool == null) { yield break; }

            var e = GameController.RemoveTokensFromPool(pool, tokens, cardSource: source);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        private IEnumerator ResetFocus()
        {
            var pool = CharacterCard.FindTokenPool("FocusPool");
            if (pool == null) { yield break; }

            
            var e = GameController.RemoveTokensFromPool(pool, pool.CurrentValue, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        public override TurnPhase AskIfTurnPhaseShouldBeChanged(TurnPhase fromPhase, TurnPhase toPhase)
        {
            Debug.Log("AskIfTurnPhaseShouldBeChanged");

            if (Card.IsFlipped) { return null; }

            if (fromPhase == null || toPhase == null) {
                Debug.Log("From or To is null, returning null");
                return null;
            }

            Debug.Log($"From phase value: {fromPhase.Phase} toPhase {toPhase.Phase}");

            if (fromPhase.TurnTaker != TurnTaker) {
                Debug.Log("One of the from/to isn't Dragon, not changing phase");
                return null;
            }
            if (fromPhase.IsBeforeStart || fromPhase.IsAfterEnd) {
                Debug.Log("Returning null for beforeStart/afterEnd");
                return null;
            }
            if (toPhase.IsBeforeStart || toPhase.IsAfterEnd) {
                Debug.Log("Returning null for beforeStart/afterEnd");
                return null;
            }

            if (toPhase.TurnTaker == TurnTaker)
            {
                // From dragon -> to dragon implies we've had our first phase.
                return new TurnPhase(HeroTurnTaker, Phase.Unknown);
            }

            if (fromPhase.Phase == Phase.Unknown && toPhase.TurnTaker != TurnTaker)
            {
                // From dragon -> to someone else means we've just had our focus phase
                return new TurnPhase(HeroTurnTaker, GameController.FindLastPhase(TurnTaker));
            }

            return null;
        }
    }
}
