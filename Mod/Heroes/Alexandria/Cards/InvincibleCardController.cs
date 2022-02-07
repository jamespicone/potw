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
            if (!preventInfo.HasValue || preventInfo.Value.identifier != dda.InstanceIdentifier)
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

                if (DidDiscardCards(discardResult))
                {
                    Card cardToDiscard = null;
                    if (discardResult.Any(dca => dca.IsPretend))
                    {
                        cardToDiscard = discardResult.First().CardToDiscard;
                    }

                    preventInfo = new PreventInfo
                    {
                        toDiscard = cardToDiscard,
                        identifier = dda.InstanceIdentifier
                    };
                }
                else
                {
                    preventInfo = null;
                }
            }

            if (preventInfo.HasValue)
            {
                IEnumerator e;
                if (preventInfo.Value.toDiscard != null)
                {
                    e = GameController.DiscardCard(
                        HeroTurnTakerController,
                        preventInfo.Value.toDiscard,
                        null,
                        TurnTaker,
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

                e = GameController.CancelAction(dda, isPreventEffect: true, cardSource: GetCardSource());
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
                preventInfo = null;
            }
        }

        private struct PreventInfo
        {
            public Card toDiscard;
            public System.Guid identifier;
        };

        private PreventInfo? preventInfo;
    }
}
