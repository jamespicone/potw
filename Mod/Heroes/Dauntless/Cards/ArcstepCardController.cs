using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dauntless
{
    public class ArcstepCardController : CardController
    {
        public ArcstepCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            AddThisCardControllerToList(CardControllerListType.ChangesVisibility);

            SpecialStringMaker.ShowSpecialString(() => $"{Card.Title} will draw {DrawAmount()} cards");
            SpecialStringMaker.ShowListOfCardsNextToCard(Card);
        }

        public override void AddTriggers()
        {
            AddTrigger<DestroyCardAction>(dca => dca.CardToDestroy.Card == Card, dca => ReturnToHand(dca), TriggerType.CancelAction, TriggerTiming.Before);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator e;

            // plasma core
            // before using Arcstep's power regain 1 HP
            if (Card.HasPlasmaCore())
            {
                e = GameController.GainHP(CharacterCard, 1, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }
            }

            // "Draw X cards, where X = 1 + the number of Charge cards attached to this card / 2 (round down)
            e = DrawCards(HeroTurnTakerController, DrawAmount());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            // You may play a card
            e = SelectAndPlayCardFromHand(HeroTurnTakerController);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            // matter to energy
            // after using Arcstep's power {DauntlessCharacter} deals up to 3 targets 1 energy damage
            if (Card.HasMatterToEnergy())
            {
                e = GameController.SelectTargetsAndDealDamage(
                    HeroTurnTakerController,
                    new DamageSource(GameController, CharacterCard),
                    amount: 1,
                    DamageType.Energy,
                    numberOfTargets: 3,
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
        }

        public override bool? AskIfCardIsVisibleToCardSource(Card card, CardSource cardSource)
        {
            // This card cannot be affected by villain or environment cards
            if (card != Card) { return null; }
            if (cardSource.Card == null) { return null; }

            if (cardSource.Card.IsVillain || cardSource.Card.IsEnvironment) { return false; }

            return null;
        }

        private int DrawAmount()
        {
            return 1 + this.ChargeCount() / 2;
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
