using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dragon
{
    public class MelusineCardController : MechCardController
    {
        public MelusineCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        protected override int FocusCost() { return 1; }

        protected override void AddExtraTriggers()
        {
            // Whenever a target deals this card damage, this card deals that target 2 fire damage
            AddCounterDamageTrigger(
                dda => dda.DamageSource.IsTarget,
                () => Card,
                () => Card,
                oncePerTargetPerTurn: false,
                damageAmount: 2,
                DamageType.Fire
            );
        }

        protected override IEnumerator HandleOtherAbilities(CardDefinition.ActivatableAbilityDefinition definition)
        {
            if (definition.Name != "focus") { yield break; }

            IEnumerator e;
            switch(definition.Number)
            {
                default: yield break;

                case 1:
                    // This card deals a target 3 melee damage
                    e = GameController.SelectTargetsAndDealDamage(
                        HeroTurnTakerController,
                        new DamageSource(GameController, Card),
                        amount: 3,
                        DamageType.Melee,
                        numberOfTargets: 1,
                        optional: false,
                        requiredTargets: 1,
                        cardSource: GetCardSource()
                    );
                    break;

                case 2:
                    // Restore this card to full HP
                    e = GameController.SetHP(Card, Card.MaximumHitPoints ?? 0, GetCardSource());
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
    }
}
