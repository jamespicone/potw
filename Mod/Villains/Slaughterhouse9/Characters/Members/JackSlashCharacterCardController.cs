using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Slaughterhouse9
{
    public class JackSlashCharacterCardController : Slaughterhouse9MemberCharacterCardController
    {
        public JackSlashCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        public override void AddSideTriggers()
        {
            if (Card.IsFlipped)
            {
                // "Whenever a hero uses a power this card deals that hero 1 melee damage"
                AddSideTrigger(AddTrigger<UsePowerAction>(upa => true, upa => AttackHero(upa), TriggerType.DealDamage, TriggerTiming.After));
            }
            else
            {
                // "The first time an Attack card would enter the trash each turn this card deals X melee damage to each hero, where X = 2 * the number of Powers controlled by that player",
                AddSideTrigger(AddAttackTrigger(() => AttackPowers(), new TriggerType[] { TriggerType.DealDamage }, "JackAttack"));

                // "The first time a Special card would enter the trash each turn play the top card of the villain deck"
                AddSideTrigger(AddSpecialTrigger(() => PlayTheTopCardOfTheVillainDeckResponse(null), new TriggerType[] { TriggerType.PlayCard }, "JackSpecial"));
            }
        }

        private IEnumerator AttackHero(UsePowerAction upa)
        {
            var selectedCard = new List<Card>();
            var e = FindCharacterCardToTakeDamage(GameController.ActiveTurnTaker, selectedCard, Card, 1, DamageType.Melee);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            if (selectedCard.Count <= 0) { yield break; }

            e = DealDamage(
                Card,
                selectedCard.First(),
                1,
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

        private IEnumerator AttackPowers()
        {
            yield break;
        }
    }
}
