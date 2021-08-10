 using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Jp.ParahumansOfTheWormverse.Alexandria
{
    public class InvincibleCardController : CardController
    {
        public InvincibleCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            AllowFastCoroutinesDuringPretend = false;
        }

        public override IEnumerator Play()
        {
            // "From when this card enters play until the start of your next turn, when {AlexandriaCharacter} would take damage you may discard a card. If you do, prevent the damage.",
            var effect = new OnDealDamageStatusEffect(
                Card,
                nameof(DiscardToPreventDamage),
                $"{HeroTurnTaker.NameRespectingVariant} may discard a card to prevent being damaged",
                new TriggerType[] { TriggerType.DiscardCard, TriggerType.CancelAction },
                TurnTaker,
                Card
            );

            effect.DamageAmountCriteria.GreaterThan = 0;
            effect.TargetCriteria.IsSpecificCard = CharacterCard;
            effect.UntilStartOfNextTurn(TurnTaker);
            effect.UntilTargetLeavesPlay(CharacterCard);

            var e = AddStatusEffect(effect);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        public override void AddTriggers()
        {
            // "Reduce damage dealt to {AlexandriaCharacter} by 1"
            AddReduceDamageTrigger(c => c == CharacterCard, 1);
        }

        public IEnumerator DiscardToPreventDamage(DealDamageAction dda, TurnTaker hero, StatusEffect effect, int[] powerNumerals = null)
        {
            Debug.Log($"Invincible discard pretend? {dda.IsPretend}, prevent? {preventDamage}");
            if (GameController.PretendMode || preventDamage == null)
            {
                var discardResult = new List<DiscardCardAction>();
                var e = SelectAndDiscardCards(
                    HeroTurnTakerController,
                    numberOfCardsToDiscard: 1,
                    optional: true,
                    gameAction: dda,
                    extraInfo: () => "Discard a card to prevent the damage",
                    storedResults: discardResult
                );
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }

                preventDamage = DidDiscardCards(discardResult);
                Debug.Log($"Invincible discard decision made pretend? {dda.IsPretend}, prevent? {preventDamage}");
            }

            if (preventDamage.GetValueOrDefault(false))
            {
                Debug.Log($"Invincible discard preventing damage pretend? {dda.IsPretend}, prevent? {preventDamage}");
                var e = GameController.CancelAction(dda, isPreventEffect: true, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }
            }

            if (! GameController.PretendMode)
            {
                preventDamage = null;
                Debug.Log($"Invincible discard setting pretend to null pretend? {dda.IsPretend}, prevent? {preventDamage}");
            }
        }

        private bool? preventDamage;
    }
}
