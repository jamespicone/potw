using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dragon
{
    public class GlaurungCardController : MechCardController
    {
        public GlaurungCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        protected override int FocusCost() { return 1; }

        protected override void AddExtraTriggers()
        {
            AddEndOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => AddDrones(),
                TriggerType.AddTokensToPool
            );

            AddTrigger<DealDamageAction>(
                dda => dda.Target == Card && dda.DidDealDamage,
                dda => LoseDrones(),
                TriggerType.ModifyTokens,
                TriggerTiming.After
            );
        }

        protected override IEnumerator HandleOtherAbilities(CardDefinition.ActivatableAbilityDefinition definition)
        {
            if (definition.Name != "focus") { yield break; }
            if (definition.Number != 1) { yield break; }

            var pool = CharacterCard.FindTokenPool("DronePool");
            if (pool == null) { yield break; }
            if (pool.CurrentValue <= 0) { yield break; }

            // This card deals up to X targets 2 lightning damage, where X is the number of Drone tokens on this card
            var e = GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, Card),
                amount: 2,
                DamageType.Lightning,
                pool.CurrentValue,
                optional: false,
                requiredTargets: 0,
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

        private IEnumerator AddDrones()
        {
            var pool = CharacterCard.FindTokenPool("DronePool");
            if (pool == null) { yield break; }

            var e = GameController.AddTokensToPool(pool, 1, GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        private IEnumerator LoseDrones()
        {
            var pool = CharacterCard.FindTokenPool("DronePool");
            if (pool == null) { yield break; }

            var e = GameController.RemoveTokensFromPool(pool, 2, cardSource: GetCardSource());
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
