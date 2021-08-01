using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Alexandria
{
    public class ProtectorCardController : CardController
    {
        public ProtectorCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // "When this card enters play {AlexandriaCharacter} regains 2 HP",
            var e = GameController.GainHP(CharacterCard, 2, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        public override void AddTriggers()
        {
            // "At the end of your turn select a target. Until the start of your next turn, whenever that target would take damage, redirect that damage to {AlexandriaCharacter}"
            AddEndOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => ProtectTarget(),
                TriggerType.Other
            );
        }

        private IEnumerator ProtectTarget()
        {
            var selectedTarget = new List<SelectCardDecision>();
            var e = GameController.SelectCardAndStoreResults(
                HeroTurnTakerController,
                SelectionType.RedirectDamageDirectedAtTarget,
                new LinqCardCriteria(c => c.IsTarget && c.IsInPlay, "target"),
                storedResults: selectedTarget,
                optional: false,
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

            var target = GetSelectedCard(selectedTarget);
            if (target == null) { yield break; }

            var effect = new OnDealDamageStatusEffect(
                CardWithoutReplacements,
                nameof(RedirectResponse), "When " + target.Title + " would be dealt damage, it is redirected to " + HeroTurnTakerController.Name,
                new TriggerType[] { TriggerType.RedirectDamage, TriggerType.WouldBeDealtDamage },
                TurnTaker,
                Card
            );

            // "Until the start of your next turn..."
            effect.UntilStartOfNextTurn(TurnTaker);

            // "... whenever that target would be dealt damage..."
            effect.TargetCriteria.IsSpecificCard = target;
            effect.DamageAmountCriteria.GreaterThan = 0;

            effect.UntilTargetLeavesPlay(target);
            effect.BeforeOrAfter = BeforeOrAfter.Before;

            e = GameController.AddStatusEffect(effect, showMessage: true, GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        public IEnumerator RedirectResponse(DealDamageAction dd, TurnTaker hero, StatusEffect effect, int[] powerNumerals = null)
        {
            // redirect that damage to {AlexandriaCharacter}
            var e = GameController.RedirectDamage(dd, CharacterCard, cardSource: GetCardSource());
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
