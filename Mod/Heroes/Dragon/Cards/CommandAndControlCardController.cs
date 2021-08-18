using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Dragon
{
    public class CommandAndControlCardController : CardController
    {
        public CommandAndControlCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator ActivateAbilityEx(CardDefinition.ActivatableAbilityDefinition ability)
        {
            if (ability.Name != "focus") { yield break; }

            var storedTargets = new List<SelectTargetDecision>();
            // 1 hero target deals a target either 2 projectile or 2 melee damage
            var e = GameController.SelectTargetAndStoreResults(
                HeroTurnTakerController,
                FindCardsWhere(new LinqCardCriteria(c => c.IsHeroTarget())),
                storedTargets,
                selectionType: SelectionType.CardToDealDamage,
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

            if (storedTargets.Count <= 0) { yield break; }
            var selectedTarget = storedTargets.First().SelectedCard;
            if (selectedTarget == null) { yield break; }

            var storedDamage = new List<SelectDamageTypeDecision>();
            e = GameController.SelectDamageType(
                HeroTurnTakerController,
                storedDamage,
                new DamageType[] { DamageType.Projectile, DamageType.Melee },
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

            if (storedDamage.Count() <= 0) { yield break; }
            var selectedDamage = storedDamage.First().SelectedDamageType;
            if (selectedDamage == null) { yield break; }

            e = GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, selectedTarget),
                2,
                selectedDamage.Value,
                1,
                optional: false,
                requiredTargets: 1,
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
