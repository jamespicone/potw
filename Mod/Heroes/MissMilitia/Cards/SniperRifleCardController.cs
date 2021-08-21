using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.MissMilitia
{
    public class SniperRifleCardController : WeaponCardController
    {
        public SniperRifleCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController, WeaponType.SniperRifle)
        {
            ShowWeaponStatusIfActive(WeaponType.Pistol);
        }

        protected override IEnumerator DoWeaponEffect(bool activateAll)
        {
            int amount = GetPowerNumeral(0, 4);
            int draws = GetPowerNumeral(1, 3);
            
            // "Discard a card."
            var e = GameController.SelectAndDiscardCard(
                HeroTurnTakerController,
                responsibleTurnTaker: TurnTaker,
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

            // "Select a target. At the start of your next turn, {MissMilitiaCharacter} deals that target 4 projectile damage."
            var choices = new List<SelectTargetDecision>();
            IEnumerable<Card> targets = GameController.FindTargetsInPlay();
            e = GameController.SelectTargetAndStoreResults(
                HeroTurnTakerController,
                GameController.FindTargetsInPlay(),
                choices,
                damageSource: CharacterCard,
                damageAmount: c => amount,
                damageType: DamageType.Projectile,
                selectionType: SelectionType.SelectTargetNoDamage,
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

            var targeted = choices.FirstOrDefault()?.SelectedCard;
            if (targeted != null)
            {
                var effect = new DelayedDamageStatusEffect(
                    CardWithoutReplacements,
                    nameof(ShootResponse),
                    $"At the start of her next turn, {CharacterCard.Title} will deal {amount} projectile damage to {targeted.Title}.",
                    Card
                );

                effect.DealDamageToTargetAtStartOfNextTurn(TurnTaker, targeted, amount);
                e = GameController.AddStatusEffect(effect, true, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }
            }

            // "{pistol} Draw 3 cards."
            if (activateAll || this.ShouldActivateWeaponAbility(WeaponType.Pistol))
            {
                e = GameController.DrawCards(HeroTurnTakerController, draws, cardSource: GetCardSource());
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

        public IEnumerator ShootResponse(PhaseChangeAction pca, OnPhaseChangeStatusEffect sourceEffect)
        {
            var e = this.DoDelayedDamage(sourceEffect);
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
