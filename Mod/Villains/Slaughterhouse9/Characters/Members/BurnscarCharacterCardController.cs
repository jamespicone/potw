using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Slaughterhouse9
{
    public class BurnscarCharacterCardController : Slaughterhouse9MemberCharacterCardController
    {
        public BurnscarCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        public override void AddSideTriggers()
        {
            if (Card.IsFlipped)
            {
                AddSideTrigger(AddDealDamageAtEndOfTurnTrigger(
                    TurnTaker,
                    Card,
                    c => !c.IsVillain && c.IsTarget,
                    TargetType.All,
                    1,
                    DamageType.Fire
                ));
            }
            else
            {
                // The first time a Special card would enter the trash this turn destroy all environment cards, then Burnscar deals X fire damage to all non-villain targets, where X = 1 + the number of environment cards destroyed in this way
                AddSideTrigger(AddSpecialTrigger(() => BlowUpEnvironment(), new TriggerType[] { TriggerType.DestroyCard, TriggerType.DealDamage }, "BurnscarDestroyEnvironment"));

                // "The first time an Attack card would enter the trash this turn this card deals 3 fire damage to the hero target with the lowest HP, then destroy a noncharacter hero card"
                AddSideTrigger(AddAttackTrigger(() => DealFireDamage(), new TriggerType[] { TriggerType.DestroyCard, TriggerType.DealDamage }, "BurnscarDealDamage"));
            }
        }

        private IEnumerator BlowUpEnvironment()
        {
            var destroyActions = new List<DestroyCardAction>();
            var e = GameController.DestroyCards(
                DecisionMaker,
                new LinqCardCriteria(c => c.IsEnvironment, "environment"),
                storedResults: destroyActions,
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

            e = DealDamage(
                Card,
                c => !c.IsVillain && c.IsTarget,
                1 + destroyActions.Count(dca => dca.WasCardDestroyed && dca.CardToDestroy.Card.IsEnvironment),
                DamageType.Fire
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

        private IEnumerator DealFireDamage()
        {
            var e = DealDamageToLowestHP(
                Card,
                ranking: 1,
                c => c.IsHero && c.IsTarget,
                c => 3,
                DamageType.Fire
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            e = GameController.SelectAndDestroyCard(
                DecisionMaker,
                new LinqCardCriteria(c => c.IsHero && !c.IsCharacter, "non-character hero card"),
                optional: false,
                responsibleCard: Card,
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
    }
}
