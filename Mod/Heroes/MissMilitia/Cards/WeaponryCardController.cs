using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.MissMilitia
{
    public class WeaponryCardController : MissMilitiaUtilityCardController
    {
        public WeaponryCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            ShowWeaponStatusIfActive(SubmachineGunKey);
            ShowWeaponStatusIfActive(MacheteKey);
            ShowWeaponStatusIfActive(PistolKey);
            ShowWeaponStatusIfActive(SniperRifleKey);
        }

        public override IEnumerator Play()
        {
            // "{smg} Increase damage dealt by {MissMilitiaCharacter} this turn by 1."
            if (HasUsedWeaponSinceStartOfLastTurn(SubmachineGunKey))
            {
                IncreaseDamageStatusEffect increaseStatus = new IncreaseDamageStatusEffect(1);
                increaseStatus.SourceCriteria.IsSpecificCard = base.CharacterCard;
                increaseStatus.UntilCardLeavesPlay(base.CharacterCard);
                increaseStatus.UntilThisTurnIsOver(base.Game);
                IEnumerator statusCoroutine = base.GameController.AddStatusEffect(increaseStatus, true, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(statusCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(statusCoroutine);
                }
            }
            // "{machete} {MissMilitiaCharacter} may deal a non-hero target 1 irreducible melee damage."
            if (HasUsedWeaponSinceStartOfLastTurn(MacheteKey))
            {
                IEnumerator damageCoroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), 1, DamageType.Melee, 1, false, 0, true, additionalCriteria: (Card c) => !c.IsHero, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(damageCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(damageCoroutine);
                }
            }
            // "{pistol} Draw a card."
            if (HasUsedWeaponSinceStartOfLastTurn(PistolKey))
            {
                IEnumerator drawCoroutine = base.GameController.DrawCard(base.HeroTurnTaker, false, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(drawCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(drawCoroutine);
                }
            }
            // "{sniper} You may destroy a non-character card non-hero target."
            if (HasUsedWeaponSinceStartOfLastTurn(SniperRifleKey))
            {
                IEnumerator destroyCoroutine = base.GameController.SelectAndDestroyCard(base.HeroTurnTakerController, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsTarget && !c.IsHero && !c.IsCharacter && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "non-character card non-hero targets", false, false, "non-character card non-hero target", "non-character card non-hero targets"), true, responsibleCard: base.Card, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(destroyCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(destroyCoroutine);
                }
            }
            yield break;
        }
    }
}
