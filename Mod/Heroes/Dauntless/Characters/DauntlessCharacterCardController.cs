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
            SpecialStringMaker.ShowSpecialString(() => $"Arclance will deal {CalculateArclanceDamage()} damage");
            SpecialStringMaker.ShowListOfCardsNextToCard(Card);
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator e;
            var storedResults = new List<SelectCardDecision>();

            switch(index)
            {
                case 0:
                    // One player may draw a card now
                    e = GameController.SelectHeroToDrawCard(HeroTurnTakerController, cardSource: GetCardSource());
                    break;

                case 1:
                    // Reduce all energy damage dealt to hero targets by 1 until your next turn
                    var reduceEffect = new ReduceDamageStatusEffect(1);
                    reduceEffect.DamageTypeCriteria.AddType(DamageType.Energy);
                    reduceEffect.TargetCriteria.IsHero = true;
                    reduceEffect.UntilStartOfNextTurn(TurnTaker);
                    
                    e = AddStatusEffect(reduceEffect);
                    break;

                case 2:
                    // Select an equipment card.
                    e = GameController.SelectCardAndStoreResults(
                        HeroTurnTakerController,
                        SelectionType.MakeIndestructible,
                        new LinqCardCriteria(c => c.DoKeywordsContain("equipment") && c.IsInPlay),
                        storedResults,
                        optional: false,
                        cardSource: GetCardSource()
                    );
                    break;

                default: yield break;
            }

            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            if (index == 2)
            {
                var selected = GetSelectedCard(storedResults);
                if (selected == null) { yield break; }

                // ...Until your next turn it is indestructible
                var indestructibleEffect = new MakeIndestructibleStatusEffect();
                indestructibleEffect.CardsToMakeIndestructible.IsSpecificCard = selected;
                indestructibleEffect.UntilStartOfNextTurn(TurnTaker);
                indestructibleEffect.UntilCardLeavesPlay(selected);

                e = AddStatusEffect(indestructibleEffect);
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
