using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Armsmaster
{
    [Serializable]
    public class StasisEffectorStatusEffect : OnPhaseChangeStatusEffect
    {
        public StasisEffectorStatusEffect(Card cardWithMethod, string nameOfMethod, Card cardSource)
            : base(cardWithMethod, nameOfMethod, "", new TriggerType[] { TriggerType.CancelAction }, cardSource)
        {
            NumberOfUses = 1;
        }

        public override bool IsSameAs(StatusEffect other)
        {
            return other is StasisEffectorStatusEffect;
        }

        public override void CombineWithStatusEffect(StatusEffect se)
        {
            NumberOfUses += se.NumberOfUses;
        }

        public override bool CombineWithExistingInstance => true;

        public override string ToString()
        {
            if (NumberOfUses == 1)
            {
                return "The next time a villain card would be played prevent it";
            }
            else
            {
                return $"The next {NumberOfUses.Value} times a villain card would be played prevent it";
            }
        }
    };

    public class StasisEffectorCardController : ModuleCardController
    {
        public StasisEffectorCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            AddTrigger<PlayCardAction>(
                pca => 
                    pca.CardToPlay.Is(this).Villain() && 
                    !pca.IsPutIntoPlay && 
                    StatusEffectIsActive() &&
                    GameController.IsCardLocationVisibleToSource(FindCardController(pca.CardToPlay), GetCardSource()) &&
                    (pca.BattleZone == null || pca.BattleZone == BattleZone),
                pca => PreventCardPlay(pca),
                TriggerType.CancelAction,
                TriggerTiming.Before,
                outOfPlayTrigger: true
            );
        }

        private bool StatusEffectIsActive()
        {
            return GameController.StatusEffectManager.StatusEffectControllers.Where(sec => sec.StatusEffect.CardSource == Card).Count() > 0;
        }

        private IEnumerator PreventCardPlay(PlayCardAction pca)
        {
            var relevantEffect = GameController.StatusEffectManager.StatusEffectControllers.Where(sec => sec.StatusEffect.CardSource == Card).FirstOrDefault();
            if (relevantEffect == null) { yield break; }

            var e = CancelAction(pca);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            if (pca.IsPretend) { yield break; }

            relevantEffect.StatusEffect.NumberOfUses--;
            if (relevantEffect.StatusEffect.NumberOfUses <= 0)
            {
                e = GameController.ExpireStatusEffect(relevantEffect.StatusEffect, GetCardSource());
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

        public override IEnumerator DoPrimary()
        {
            // Discard 2 cards. If you do, the next time a villain card would be played prevent it; and until the start of your next turn all villain character cards are immune to damage
            var discardResults = new List<DiscardCardAction>();
            var e = SelectAndDiscardCards(
                HeroTurnTakerController,
                2,
                storedResults: discardResults,
                responsibleTurnTaker: TurnTaker
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            if (GetNumberOfCardsDiscarded(discardResults) < 2) { yield break; }

            var cannotPlayCardsEffect = new StasisEffectorStatusEffect(Card, nameof(NoOpEffect), Card);
            e = AddStatusEffect(cannotPlayCardsEffect);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var villainDamageImmunityEffect = new ImmuneToDamageStatusEffect();
            villainDamageImmunityEffect.UntilStartOfNextTurn(TurnTaker);
            villainDamageImmunityEffect.TargetCriteria.IsVillain = true;
            villainDamageImmunityEffect.TargetCriteria.IsCharacter = true;
            villainDamageImmunityEffect.CardSource = Card;

            e = AddStatusEffect(villainDamageImmunityEffect);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        public override IEnumerator DoSecondary()
        {
            // Destroy a target with 4 or less HP
            var e = GameController.SelectAndDestroyCard(HeroTurnTakerController, new LinqCardCriteria(c => c.IsTarget && c.HitPoints <= 4), optional: false, responsibleCard: Card, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        public IEnumerator NoOpEffect(PhaseChangeAction p, OnPhaseChangeStatusEffect effect)
        {
            yield break;
        }
    }
}
