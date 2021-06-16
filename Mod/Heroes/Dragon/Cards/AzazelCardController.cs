using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dragon
{
    public class AzazelCardController : MechCardController
    {
        public AzazelCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        protected override int FocusCost() { return 2; }

        protected override void AddExtraTriggers()
        {
            AddReduceDamageTrigger(c => c == CharacterCard, 1);

            AddDealDamageAtEndOfTurnTrigger(
                TurnTaker,
                Card,
                c => true,
                TargetType.SelectTarget,
                amount: 2,
                DamageType.Energy,
                isIrreducible: true,
                optional: true
            );
        }

        public override IEnumerator ActivateAbilityEx(CardDefinition.ActivatableAbilityDefinition definition)
        {
            if (definition.Name != "focus") { yield break; }

            switch(definition.Number)
            {
                default: yield break;

                case 1:
                    var e = GameController.SelectTargetsAndDealDamage(
                        HeroTurnTakerController,
                        new DamageSource(GameController, Card),
                        amount: 2,
                        DamageType.Energy,
                        numberOfTargets: 1,
                        optional: false,
                        requiredTargets: 1,
                        isIrreducible: true,
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
                    yield break;
            }
        }
    }
}
