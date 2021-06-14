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
                    pca => true, pca => DoFocusActions(), new TriggerType[] { TriggerType.UsePower }, TriggerTiming.After ));
            }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            yield break;
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

        public override IEnumerator ActivateAbilityEx(CardDefinition.ActivatableAbilityDefinition ability)
        {
            Debug.Log("ActivateAbilityEx with name \"" + ability.Name + "\" and number " + ability.Number);

            if (ability.Name == "focus")
            {
                IEnumerator e;
                switch(ability.Number)
                {
                    case 0:
                        // Draw a card
                        e = DrawCard(HeroTurnTaker);
                        break;

                    case 1:
                        // Play a card
                        e = SelectAndPlayCardFromHand(HeroTurnTakerController);
                        break;

                    default:
                        Debug.Log("Unrecognised ability");
                        yield break;
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
            else
            {
                yield return base.ActivateAbilityEx(ability);
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
            if (Card.IsFlipped) { return null; }

            if (fromPhase == null || toPhase == null) { return null; }
            if (fromPhase.TurnTaker != TurnTaker || toPhase.TurnTaker != TurnTaker) { return null; }
            if (fromPhase.IsBeforeStart || fromPhase.IsAfterEnd) { return null; }
            if (toPhase.IsBeforeStart || toPhase.IsAfterEnd) { return null; }

            if (fromPhase.Phase != Phase.Unknown)
            {
                return new TurnPhase(HeroTurnTaker, Phase.Unknown);
            }
            else
            {
                return new TurnPhase(HeroTurnTaker, Phase.End);
            }
        }
    }
}
