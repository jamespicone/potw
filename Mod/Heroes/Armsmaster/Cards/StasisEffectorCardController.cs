using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Jp.ParahumansOfTheWormverse.Armsmaster
{
    public class StasisEffectorCardController : ModuleCardController
    {
        public StasisEffectorCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

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

            // This doesn't work because the engine doesn't appear to use NumberOfUses for
            // CannotPlayCardsStatusEffect. It looks like it does in code so it must be a dead
            // path or something.

            // TODO: Do a 'manual status effect' with an out-of-play trigger that relies on a status effect being present

            //var cannotPlayCardsEffect = new CannotPlayCardsStatusEffect();

            //cannotPlayCardsEffect.CardCriteria.IsVillain = true;
            //cannotPlayCardsEffect.NumberOfUses = 1;
            //cannotPlayCardsEffect.CardSource = Card;

            var cannotPlayCardsEffect = new OnPhaseChangeStatusEffect(
                Card,
                nameof(HandlePreventVillainCards),
                $"Skip the next villain play phase",
                new[] { TriggerType.PreventPhaseAction },
                Card
            );
            cannotPlayCardsEffect.TurnTakerCriteria.IsVillain = true;
            cannotPlayCardsEffect.TurnPhaseCriteria.Phase = Phase.PlayCard;
            cannotPlayCardsEffect.CanEffectStack = true;
            cannotPlayCardsEffect.CardSource = Card;
            cannotPlayCardsEffect.NumberOfUses = 1;

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

        public IEnumerator HandlePreventVillainCards(PhaseChangeAction phase, OnPhaseChangeStatusEffect sourceEffect)
        {
            // TODO: This seems to fire even if some other effect has cancelled the turn,
            // even if it's another instance of the status effect (or at least it expires).
            //
            // Not sure why.
            //
            if (!phase.ToPhase.IsVillain) { yield break; }
            if (!phase.ToPhase.IsPlayCard) { yield break; }
            if (!phase.ToPhase.CanPerformAction) { yield break; }
            if (phase.ToPhase.WasSkipped) { yield break; }
            if (phase.ToPhase.PhaseActionCountRoot == null) { yield break; }

            var turnsLeft = (phase.ToPhase.PhaseActionCountRoot ?? 0) + (phase.ToPhase.PhaseActionCountModifiers ?? 0);
            if (turnsLeft <= 0) { yield break; }

            sourceEffect.NumberOfUses--;

            if (sourceEffect.NumberOfUses < 0)
            {
                sourceEffect.UntilEndOfPhase(phase.ToPhase.TurnTaker, phase.ToPhase.Phase);
            }

            var e = GameController.PreventPhaseAction(phase.ToPhase, cardSource: GetCardSource());
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
