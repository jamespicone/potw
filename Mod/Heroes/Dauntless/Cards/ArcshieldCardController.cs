using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dauntless
{
    public class ArcshieldCardController : CardController
    {
        public ArcshieldCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            AddThisCardControllerToList(CardControllerListType.ChangesVisibility);

            SpecialStringMaker.ShowSpecialString(() => $"{Card.Title} will reduce damage by {ReductionAmount()}");
            SpecialStringMaker.ShowListOfCardsNextToCard(Card);
        }

        public override void AddTriggers()
        {
            // Plasma Core
            // whenever {DauntlessCharacter} is dealt damage and it is reduced by Arcshield's power {DauntlessCharacter} deals the source 1 energy damage
            AddCounterDamageTrigger(
                dda => DidArcshieldReduce(dda),
                () => CharacterCard,
                () => CharacterCard,
                oncePerTargetPerTurn: false,
                damageAmount: 1,
                DamageType.Energy
            );

            AddTrigger<DestroyCardAction>(dca => dca.CardToDestroy.Card == Card, dca => ReturnToHand(dca), TriggerType.CancelAction, TriggerTiming.Before);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // "Until the end of your next turn reduce damage dealt to Dauntless by X, where X = 1 + the number of Charge cards attached to this card / 2 (round down)
            var reduceEffect = new ReduceDamageStatusEffect(ReductionAmount());
            reduceEffect.TargetCriteria.IsSpecificCard = CharacterCard;
            reduceEffect.TargetLeavesPlayExpiryCriteria.Card = CharacterCard;
            reduceEffect.CardSource = Card;
            reduceEffect.UntilEndOfNextTurn(TurnTaker);

            var e = AddStatusEffect(reduceEffect);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            // Matter To Energy
            // after using Arcshield's power pick a damage type; reduce all damage of that type by 1 until the end of your next turn
            if (Card.HasMatterToEnergy())
            {
                var storedResult = new List<SelectDamageTypeDecision>();
                e = GameController.SelectDamageType(HeroTurnTakerController, storedResult, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }

                var damageType = GetSelectedDamageType(storedResult);

                if (damageType.HasValue)
                {
                    reduceEffect = new ReduceDamageStatusEffect(1);
                    reduceEffect.DamageTypeCriteria.AddType(damageType.Value);
                    reduceEffect.CardSource = Card;
                    reduceEffect.UntilEndOfNextTurn(TurnTaker);

                    e = AddStatusEffect(reduceEffect);
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

        public override bool? AskIfCardIsVisibleToCardSource(Card card, CardSource cardSource)
        {
            // This card cannot be affected by villain or environment cards
            if (card != Card) { return null; }
            if (cardSource.Card == null) { return null; }

            if (cardSource.Card.IsVillain || cardSource.Card.IsEnvironment) { return false; }

            return null;
        }

        private int ReductionAmount()
        {
            return 1 + this.ChargeCount() / 2;
        }

        private bool DidArcshieldReduce(DealDamageAction dda)
        {
            return dda.DamageModifiers.Any(
                mdda =>
                    mdda.CardSource != null &&
                    mdda.CardSource.StatusEffectSource != null &&
                    mdda.CardSource.StatusEffectSource.CardSource == Card
            );
        }

        private IEnumerator ReturnToHand(DestroyCardAction dca)
        {
            var e = CancelAction(dca);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            e = GameController.MoveCard(
                TurnTakerController,
                Card,
                HeroTurnTaker.Hand,
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
