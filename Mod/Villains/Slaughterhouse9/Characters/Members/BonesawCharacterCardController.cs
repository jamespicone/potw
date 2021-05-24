using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Slaughterhouse9
{
    public class BonesawCharacterCardController : Slaughterhouse9MemberCharacterCardController
    {
        public BonesawCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        public override void AddSideTriggers()
        {
            if (Card.IsFlipped)
            {
                // At the end of the villain turn the villain target with the lowest HP regains 1 HP
                AddSideTrigger(AddEndOfTurnTrigger(tt => tt.IsVillain, pca => RestoreHP(), TriggerType.GainHP));
            }
            else
            {
                // The first time a Defence card would enter the trash each turn each villain target regains 2 HP
                AddSideTrigger(AddDefenceTrigger(() => RestoreEveryonesHP(), new TriggerType[] { TriggerType.GainHP }, "BonesawAllHP"));

                // "The first time a Special card would enter the trash each turn this card deals 2 toxic damage to all non-villain targets
                AddSideTrigger(AddSpecialTrigger(() => DealToxicDamage(), new TriggerType[] { TriggerType.DealDamage }, "BonesawToxicDamage"));
            }
        }

        private IEnumerator RestoreHP()
        {
            var lowestCard = new List<Card>();
            var e = GameController.FindTargetWithLowestHitPoints(
                ranking: 1,
                c => c.IsVillainTarget,
                lowestCard,
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

            if (lowestCard.Count <= 0)
            {
                yield break;
            }

            e = GameController.GainHP(lowestCard.First(), 1, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        private IEnumerator RestoreEveryonesHP()
        {
            var e = GameController.GainHP(
                DecisionMaker,
                c => c.IsVillainTarget,
                2,
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

        private IEnumerator DealToxicDamage()
        {
            var e = DealDamage(
                Card,
                c => ! c.IsVillain && c.IsTarget,
                2,
                DamageType.Toxic
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
