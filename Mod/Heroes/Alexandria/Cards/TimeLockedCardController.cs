using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Alexandria
{
    [Serializable]
    class TimelockedStatusEffect : OnPhaseChangeStatusEffect
    {
        public TimelockedStatusEffect(Card cardWithMethod, string nameOfMethod, TurnTaker player, Card target)
            : base(cardWithMethod, nameOfMethod, $"At the start of {player.Name}'s next turn {target.Title}'s HP will be set to {target.HitPoints}", new TriggerType[] { TriggerType.Other }, cardWithMethod)
        {
            RecordedHP = target.HitPoints ?? 0;

            UntilTargetLeavesPlay(target);

            TurnPhaseCriteria.TurnTaker = player;
            TurnPhaseCriteria.Phase = Phase.Start;
            BeforeOrAfter = BeforeOrAfter.After;
            CanEffectStack = true;
        }

        public int RecordedHP { get; private set; }
    }

    public class TimeLockedCardController : CardController
    {
        public TimeLockedCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        public override IEnumerator Play()
        {
            //"Note down Alexandria's current HP",
            if (CharacterCard.HitPoints == null) { yield break; }

            var effect = new TimelockedStatusEffect(
                CardWithoutReplacements,
                nameof(ResetHP),
                TurnTaker,
                CharacterCard
            );

            effect.TurnPhaseCriteria.TurnTaker = TurnTaker;
            effect.TurnPhaseCriteria.Phase = Phase.Start;

            var e = AddStatusEffect(effect, true);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        public IEnumerator ResetHP(PhaseChangeAction pca, StatusEffect effect)
        {
            IEnumerator e;
            do
            {
                var lockedEffect = effect as TimelockedStatusEffect;
                if (lockedEffect == null) { break; }

                var target = lockedEffect.TargetLeavesPlayExpiryCriteria.IsOneOfTheseCards.FirstOrDefault();
                if (target == null) { break; }

                // if {AlexandriaCharacter} is not incapacitated, 
                if (target.IsIncapacitatedOrOutOfGame)
                {
                    e = GameController.SendMessageAction(
                        $"{target.Title} is incapacitated or out of the game",
                        Priority.High,
                        cardSource: GetCardSource(),
                        showCardSource: true
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
                else
                {
                    // set {AlexandriaCharacter}'s HP to the value you noted down
                    e = GameController.SetHP(target, lockedEffect.RecordedHP, GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(e);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(e);
                    }
                }
            } while (false);

            e = GameController.ExpireStatusEffect(effect, GetCardSource());
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
}
