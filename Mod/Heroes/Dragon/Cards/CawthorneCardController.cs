using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dragon
{
    public class CawthorneCardController : MechCardController
    {
        public CawthorneCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        protected override int FocusCost() { return 1; }

        protected override void AddExtraTriggers()
        {
            // At the end of your turn this card deals 1 projectile damage to up to 2 targets.
            AddDealDamageAtEndOfTurnTrigger(
                TurnTaker,
                Card,
                c => true,
                TargetType.SelectTarget,
                amount: 1,
                DamageType.Projectile,
                optional: true,
                numberOfTargets: 2
            );
        }

        public override IEnumerator ActivateAbilityEx(CardDefinition.ActivatableAbilityDefinition definition)
        {
            if (definition.Name != "focus") { yield break; }
            if (definition.Number != 1) { yield break; }

            // Select up to 2 targets. Reduce the next damage those targets deal by 2
            var e = GameController.SelectCardsAndDoAction(
                HeroTurnTakerController,
                new LinqCardCriteria(c => c.IsTarget && c.IsInPlay),
                SelectionType.Custom,
                c => ReduceNextDamage(c),
                numberOfCards: 2,
                requiredDecisions: 0,
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

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            return new CustomDecisionText(
                "Select a target to deal less damage",
                "Selecting a target to deal less damage",
                "Vote for a target to deal less damage",
                "Target to deal less damage"
            );
        }

        private IEnumerator ReduceNextDamage(Card c)
        {
            var effect = new ReduceDamageStatusEffect(2);
            effect.NumberOfUses = 1;
            effect.SourceCriteria.IsSpecificCard = c;
            effect.CardDestroyedExpiryCriteria.Card = c;

            var e = GameController.AddStatusEffect(effect, showMessage: true, GetCardSource());
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
