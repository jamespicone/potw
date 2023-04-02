using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.MissMilitia
{
    public class WeaponryCardController : MissMilitiaUtilityCardController
    {
        public WeaponryCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            ShowWeaponStatusIfActive(WeaponType.SubmachineGun);
            ShowWeaponStatusIfActive(WeaponType.Machete);
            ShowWeaponStatusIfActive(WeaponType.Pistol);
            ShowWeaponStatusIfActive(WeaponType.SniperRifle);
        }

        public override IEnumerator Play()
        {
            IEnumerator e;

            // "{smg} Increase damage dealt by {MissMilitiaCharacter} this turn by 1."
            if (this.ShouldActivateWeaponAbility(WeaponType.SubmachineGun))
            {
                var increaseStatus = new IncreaseDamageStatusEffect(1);
                increaseStatus.SourceCriteria.IsSpecificCard = base.CharacterCard;
                increaseStatus.UntilCardLeavesPlay(CharacterCard);
                increaseStatus.UntilThisTurnIsOver(Game);
                e = GameController.AddStatusEffect(increaseStatus, showMessage: true, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }
            }

            // "{machete} {MissMilitiaCharacter} may deal a non-hero target 1 irreducible melee damage."
            if (this.ShouldActivateWeaponAbility(WeaponType.Machete))
            {
                e = GameController.SelectTargetsAndDealDamage(
                    HeroTurnTakerController,
                    new DamageSource(GameController, CharacterCard),
                    amount: 1,
                    DamageType.Melee,
                    numberOfTargets: 1,
                    optional: false,
                    requiredTargets: 0,
                    isIrreducible: true,
                    additionalCriteria: (c) => c.Is(this).NonHero().Target(),
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

            // "{pistol} Draw a card."
            if (this.ShouldActivateWeaponAbility(WeaponType.Pistol))
            {
                e = GameController.DrawCard(HeroTurnTaker, optional: false, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }
            }

            // "{sniper} You may destroy a non-character card non-hero target."
            if (this.ShouldActivateWeaponAbility(WeaponType.SniperRifle))
            {
                e = GameController.SelectAndDestroyCard(
                    HeroTurnTakerController,
                    new LinqCardCriteria((c) => c.IsInPlayAndHasGameText && c.Is(this).NonHero().Target() && ! c.IsCharacter && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "non-character card non-hero targets", false, false, "non-character card non-hero target", "non-character card non-hero targets"),
                    optional: true,
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
}
