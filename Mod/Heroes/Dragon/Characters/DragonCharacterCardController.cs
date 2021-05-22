using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Jp.ParahumansOfTheWormverse.Dragon
{
    // TODO:
    // This is all proof of concept. Demonstrates that the turn structure stuff can work, as far as I can tell, although not sure if unspecified wonky turn
    // orders could fuck it up.
    //
    // Issues:
    // - Token pool doesn't render. Maybe only the known pools render?
    // - ActivateAbility only takes an ability key, so cards with multiple activateable abilities don't work. I've submitted a feature request, but
    //   it *is* possible for card code to hand-hack around this by essentially implementing SelectAndActivateAbility by hand so it can take multiple ability
    //   keys and then having focus1, focus2, focus3 abilities.
    public class DragonCharacterCardController : HeroCharacterCardController
    {
        public DragonCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            AddThisCardControllerToList(CardControllerListType.ChangesPhaseOrder);
        }

        public override void AddSideTriggers()
        {
            if(! Card.IsFlipped)
            {
                AddSideTrigger(AddPhaseChangeTrigger(tt => tt == TurnTaker, p => p == Phase.Start || p == Phase.End, pca => true,
                    pca => ResetFocus(pca), new TriggerType[] { TriggerType.FirstTrigger }, TriggerTiming.Before));

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
            yield break;
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
                        Debug.Log("Draw a card");
                        e = DrawCard(HeroTurnTaker);
                        break;

                    case 1:
                        Debug.Log("Play a card");
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

        private IEnumerator ResetFocus(PhaseChangeAction pca)
        {
            if (HasBeenSetToTrueThisTurn("DragonFocusReset")) { yield break; }

            SetCardPropertyToTrueIfRealAction("DragonFocusReset");

            var pool = CharacterCard.FindTokenPool("FocusPool");
            if (pool == null) { yield break; }

            var tokensToMove = 4 - pool.CurrentValue;
            IEnumerator e;
            if (tokensToMove < 0)
            {
                e = GameController.RemoveTokensFromPool(pool, -tokensToMove, cardSource: GetCardSource());
            }
            else
            {
                e = GameController.AddTokensToPool(pool, tokensToMove, GetCardSource());
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
