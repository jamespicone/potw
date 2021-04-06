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
    public class SniperRifleCardController : WeaponCardController
    {
        public SniperRifleCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController, "{sniper}")
        {
            ShowIconStatusIfActive(PistolIcon);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int amount = GetPowerNumeral(0, 4);
            int draws = GetPowerNumeral(1, 3);
            // "Discard a card."
            IEnumerator discardCoroutine = base.GameController.SelectAndDiscardCard(base.HeroTurnTakerController, responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(discardCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(discardCoroutine);
            }
            // "Select a target. At the start of your next turn, {MissMilitiaCharacter} deals that target 4 projectile damage."
            List<SelectTargetDecision> choices = new List<SelectTargetDecision>();
            IEnumerable<Card> targets = GameController.FindTargetsInPlay();
            IEnumerator selectCoroutine = base.GameController.SelectTargetAndStoreResults(base.HeroTurnTakerController, targets, choices, damageSource: base.CharacterCard, damageAmount: (Card c) => amount, damageType: DamageType.Projectile, selectionType: SelectionType.SelectTargetNoDamage, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(selectCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(selectCoroutine);
            }
            Card targeted = choices.FirstOrDefault()?.SelectedCard;
            if (targeted != null)
            {
                OnPhaseChangeStatusEffect shootEffect = new OnPhaseChangeStatusEffect(base.Card, nameof(ShootResponse), "At the start of her next turn, " + base.CharacterCard.Title + " will deal " + amount.ToString() + " projectile damage to " + targeted.Title + ".", new TriggerType[] { TriggerType.DealDamage }, base.Card);
                shootEffect.UntilEndOfNextTurn(base.TurnTaker);
                shootEffect.TurnTakerCriteria.IsSpecificTurnTaker = base.TurnTaker;
                shootEffect.UntilTargetLeavesPlay(targeted);
                shootEffect.TurnPhaseCriteria.Phase = Phase.Start;
                shootEffect.BeforeOrAfter = BeforeOrAfter.After;
                shootEffect.CanEffectStack = true;
                shootEffect.CardSource = base.Card;
                shootEffect.NumberOfUses = 1;
                shootEffect.DoesDealDamage = true;
                shootEffect.SetPowerNumeralsArray(new int[] { amount, draws });
                IEnumerator shootCoroutine = base.GameController.AddStatusEffect(shootEffect, true, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(shootCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(shootCoroutine);
                }
            }
            // "Until the end of your next turn, you may activate {sniper} effects."
            ActivateEffectStatusEffect activateSniper = new ActivateEffectStatusEffect(base.TurnTaker, null, EffectIcon);
            activateSniper.UntilEndOfNextTurn(base.TurnTaker);
            IEnumerator statusCoroutine = base.GameController.AddStatusEffect(activateSniper, true, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(statusCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(statusCoroutine);
            }
            // "{pistol} Draw 3 cards."
            if (PistolActive)
            {
                IEnumerator drawCoroutine = base.GameController.DrawCards(base.HeroTurnTakerController, draws, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(drawCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(drawCoroutine);
                }
            }
            yield break;
        }

        public IEnumerator ShootResponse(PhaseChangeAction pca, OnPhaseChangeStatusEffect sourceEffect)
        {
            // "... {MissMilitiaCharacter} deals that target 4(?) projectile damage."
            var target = sourceEffect.TargetLeavesPlayExpiryCriteria.IsOneOfTheseCards.FirstOrDefault();
            int powerNumeral = sourceEffect.PowerNumeralsToChange[0];
            if (!base.CharacterCard.IsIncapacitatedOrOutOfGame && target.IsTarget && target.IsInPlayAndNotUnderCard)
            {
                if (GameController.IsCardVisibleToCardSource(target, new CardSource(FindCardController(base.CharacterCard))) != true)
                {
                    IEnumerator messageCoroutine = base.GameController.SendMessageAction(base.CharacterCardWithoutReplacements.Title + " can't hit " + target.Title + " from here!", Priority.Medium, GetCardSource(), new Card[] { target });
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(messageCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(messageCoroutine);
                    }
                }
                else
                {
                    IEnumerator damageCoroutine = DealDamage(base.CharacterCard, target, powerNumeral, DamageType.Projectile, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(damageCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(damageCoroutine);
                    }
                }
            }
            yield break;
        }
    }
}
