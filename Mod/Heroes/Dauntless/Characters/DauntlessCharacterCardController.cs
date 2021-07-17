using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dauntless
{
    public class DauntlessCharacterCardController : HeroCharacterCardController
    {
        public DauntlessCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            yield break;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            DamageType type = DamageType.Energy;
            if (Card.HasMatterToEnergy())
            {
                // ...choose the damage type for his power
                var storedResults = new List<SelectDamageTypeDecision>();
                var e2 = GameController.SelectDamageType(HeroTurnTakerController, storedResults, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e2);
                }
                else
                {
                    GameController.ExhaustCoroutine(e2);
                }

                if (storedResults.Count() > 0)
                {
                    type = storedResults.First().SelectedDamageType ?? DamageType.Energy;
                }
            }

            // Deal X energy damage to a target, where X = 1 + the number of Charge cards attached to Dauntless / 2 (round down)
            var e = GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, Card),
                CalculateArclanceDamage(),
                type,
                numberOfTargets: 1,
                optional: false,
                requiredTargets: 1,
                isIrreducible: Card.HasPlasmaCore(), // ...his power does irreducible damage
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

        private int CalculateArclanceDamage()
        {
            return 1 + this.ChargeCount() / 2;
        }
    }
}
