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
    public class SubmachineGunCardController : WeaponCardController
    {
        public SubmachineGunCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController, "{smg}")
        {
            ShowIconStatusIfActive(MacheteIcon);
            ShowIconStatusIfActive(SniperIcon);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int numTargets = GetPowerNumeral(0, 3);
            int amount = GetPowerNumeral(1, 1);
            // "{MissMilitiaCharacter} deals up to 3 non-hero targets 1 projectile damage each."
            IEnumerator damageCoroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), amount, DamageType.Projectile, numTargets, false, 0, additionalCriteria: (Card c) => !c.IsHero, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(damageCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(damageCoroutine);
            }
            // "Until the end of your next turn, you may activate {smg} effects."
            ActivateEffectStatusEffect activateSmg = new ActivateEffectStatusEffect(base.TurnTaker, null, EffectIcon);
            activateSmg.UntilEndOfNextTurn(base.TurnTaker);
            IEnumerator statusCoroutine = base.GameController.AddStatusEffect(activateSmg, true, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(statusCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(statusCoroutine);
            }
            // "{machete} You may destroy an Ongoing or environment card."
            if (MacheteActive)
            {
                IEnumerator destroyCoroutine = base.GameController.SelectAndDestroyCard(base.HeroTurnTakerController, new LinqCardCriteria((Card c) => c.DoKeywordsContain("ongoing") || c.IsEnvironment, "Ongoing or environment"), true, responsibleCard: base.Card, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(destroyCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(destroyCoroutine);
                }
            }
            // "{sniper} You may put a non-hero non-character target in play on top of its deck."
            if (SniperActive)
            {
                List<SelectCardDecision> cardChoices = new List<SelectCardDecision>();
                IEnumerator chooseCoroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.MoveCardOnDeck, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsTarget && !c.IsHero && !c.IsCharacter && base.GameController.IsCardVisibleToCardSource(c, GetCardSource()), "non-hero non-character targets", false, singular: "non-hero non-character target", plural: "non-hero non-character targets"), cardChoices, true, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(chooseCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(chooseCoroutine);
                }
                SelectCardDecision firstChoice = cardChoices.Where((SelectCardDecision scd) => scd.Completed).FirstOrDefault();
                if (firstChoice != null && firstChoice.SelectedCard != null)
                {
                    Card chosen = firstChoice.SelectedCard;
                    IEnumerator moveCoroutine = base.GameController.MoveCard(base.TurnTakerController, chosen, chosen.NativeDeck, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(moveCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(moveCoroutine);
                    }
                }
            }
            yield break;
        }
    }
}
