using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dragon
{
    public class AstarothNidhugCardController : MechCardController
    {
        public AstarothNidhugCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        protected override int FocusCost() { return 1; }

        protected override void AddExtraTriggers()
        {
            // "At the end of your turn place an Aim token on this card"
            AddEndOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => AddAimToken(),
                TriggerType.AddTokensToPool
            );
        }

        protected override IEnumerator HandleOtherAbilities(CardDefinition.ActivatableAbilityDefinition definition)
        {
            if (definition.Name != "focus") { yield break; }

            switch (definition.Number)
            {
                case 1:
                    // Place an Aim token on this card
                    yield return AddAimToken();
                    break;

                case 2:
                    // Deal X projectile damage to a target, where X is the number of Aim tokens on this card. Remove all Aim tokens from this card
                    yield return Shoot();
                    break;

                default:
                    yield break;
            }
        }

        private IEnumerator AddAimToken()
        {
            var pool = CharacterCard.FindTokenPool("AimPool");
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

        private IEnumerator Shoot()
        {
            // Deal X projectile damage to a target, where X is the number of Aim tokens on this card. Remove all Aim tokens from this card
            var pool = CharacterCard.FindTokenPool("AimPool");
            if (pool == null) { yield break; }

            var e = GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, Card),
                pool.CurrentValue,
                DamageType.Projectile,
                numberOfTargets: 1,
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

            e = GameController.RemoveTokensFromPool(pool, pool.CurrentValue, cardSource: GetCardSource());
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
