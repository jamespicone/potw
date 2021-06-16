using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dragon
{
    public class LadonCardController : MechCardController
    {
        public LadonCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        protected override int FocusCost() { return 1; }

        protected override void AddExtraTriggers()
        {
            // Reduce damage dealt to this card by 1
            AddReduceDamageTrigger(c => c == Card, 1);
        }

        public override IEnumerator ActivateAbilityEx(CardDefinition.ActivatableAbilityDefinition definition)
        {
            if (definition.Name != "focus") { yield break; }
            if (definition.Number != 1) { yield break; }

            // Select a damage type. Until the start of your next turn reduce all damage of that type by 1

            var selected = new List<SelectDamageTypeDecision>();
            var e = GameController.SelectDamageType(
                HeroTurnTakerController,
                selected,
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

            var damageType = GetSelectedDamageType(selected);
            if (damageType == null) { yield break; }

            var status = new ReduceDamageStatusEffect(1);
            status.DamageTypeCriteria.AddType(damageType.Value);
            status.UntilStartOfNextTurn(TurnTaker);

            e = GameController.AddStatusEffect(status, true, GetCardSource());
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