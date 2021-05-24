using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Slaughterhouse9
{
    public class CrawlerCharacterCardController : Slaughterhouse9MemberCharacterCardController
    {
        public CrawlerCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        public override void AddSideTriggers()
        {
            if (Card.IsFlipped)
            {
                // "At the end of the villain turn this card deals 2 melee damage to the hero target with the lowest HP"
                AddSideTrigger(AddDealDamageAtEndOfTurnTrigger(
                    TurnTaker,
                    Card,
                    c => c.IsHero && c.IsTarget,
                    TargetType.LowestHP,
                    2,
                    DamageType.Melee
                ));
            }
            else
            {
                // "The first time an Attack card would enter the trash this turn this card deals 2 toxic damage and 2 melee damage to the non-villain target with the highest HP.",
                AddSideTrigger(AddAttackTrigger(() => DealDamageResponse(), new TriggerType[] { TriggerType.DealDamage }, "CrawlerDamage"));

                // "The first time a Special card would enter the trash each turn this card regains 30 HP"
                AddSideTrigger(AddSpecialTrigger(() => RegenerateResponse(), new TriggerType[] { TriggerType.GainHP }, "CrawlerRegenerate"));

                // "Whenever Crawler is dealt damage he becomes immune to that damage type until the next time he takes damage."
                AddSideTrigger(AddTrigger<DealDamageAction>(dda => dda.Target == Card, dda => MaybeImmune(dda), TriggerType.ImmuneToDamage, TriggerTiming.Before));
                AddSideTrigger(AddTrigger<DealDamageAction>(dda => dda.Target == Card && dda.DidDealDamage, dda => MakeImmune(dda), TriggerType.MakeImmuneToDamage, TriggerTiming.After));
            }
        }

        private IEnumerator DealDamageResponse()
        {
            var target = new List<DealDamageAction>();
            var e = DealDamageToHighestHP(
                Card,
                1,
                c => !c.IsVillain && c.IsTarget,
                c => 2,
                DamageType.Toxic,
                storedResults: target
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var dda = target.FirstOrDefault();
            if (dda == null || dda.OriginalTarget == null) { yield break; }

            e = DealDamage(
                Card,
                dda.OriginalTarget,
                2,
                DamageType.Melee,
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

        private IEnumerator RegenerateResponse()
        {
            var e = GameController.GainHP(Card, 30, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        private IEnumerator MaybeImmune(DealDamageAction dda)
        {
            var immunity = GetCardPropertyJournalEntryInteger(DamageImmunityKey);
            if (immunity != (int)dda.DamageType)
            {
                yield break;
            }

            var e = GameController.ImmuneToDamage(dda, GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        private IEnumerator MakeImmune(DealDamageAction dda)
        {
            if (dda.IsPretend) { yield break; }
            SetCardProperty(DamageImmunityKey, (int)dda.DamageType);
        }

        private const string DamageImmunityKey = "CrawlerDamageImmunity";
    }
}
