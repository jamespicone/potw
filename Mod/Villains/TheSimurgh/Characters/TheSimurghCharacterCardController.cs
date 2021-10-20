using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.TheSimurgh
{
    public class TheSimurghCharacterCardController : CharacterCardController
    {
        public TheSimurghCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        private Location ConditionDeck => TurnTaker.FindSubDeck("ConditionDeck");

        public override bool AskIfCardIsIndestructible(Card card)
        {
            // "Face-down villain cards are indestructible."
            return card.IsFaceDownNonCharacter && card.Is().Villain().AccordingTo(this);
        }

        public override void AddSideTriggers()
        {
            if (Card.IsFlipped)
            {
                // Reduce damage dealt to {TheSimurghCharacter} by 2.
                AddReduceDamageTrigger(c => c == Card, 2);

                // At the start of the villain turn, reveal the top {H + 1} cards of the villain deck. Put the revealed cards back in ascending order of {SimurghDanger}.
                AddStartOfTurnTrigger(tt => tt == TurnTaker, pca => StackDeck(H + 1), TriggerType.RevealCard);

                // At the end of the villain turn, put a token on this card. Then, each hero target deals itself X psychic damage, where X is the number of tokens on this card.
                // TODO
            }
            else
            {
                // Reduce damage dealt to {TheSimurghCharacter} by 1
                AddReduceDamageTrigger(c => c == Card, 1);
                AddStartOfTurnTrigger(tt => tt == TurnTaker, pca => DoStartOfTurnStuff(), new TriggerType[] { TriggerType.PlayCard, TriggerType.RevealCard });
                AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => DoEndOfTurnStuff(pca), new TriggerType[] { TriggerType.DealDamage, TriggerType.FlipCard });

                if (IsGameAdvanced)
                {
                    // At the start of the villain turn, play the top card of the Condition deck.
                    AddStartOfTurnTrigger(
                        tt => tt == TurnTaker,
                        pca => GameController.PlayTopCardOfLocation(TurnTakerController, ConditionDeck, responsibleTurnTaker: TurnTaker, cardSource: GetCardSource()),
                        TriggerType.PlayCard
                    );
                }
            }
        }

        private IEnumerator DoStartOfTurnStuff(bool advanced = false)
        {
            // At the start of the villain turn, play the top card of the Condition deck.
            var e = GameController.PlayTopCardOfLocation(
                TurnTakerController,
                ConditionDeck,
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

            // Then, reveal the top {H - 1} cards of the villain deck. Put the revealed cards back in ascending order of {SimurghDanger}.",
            e = StackDeck(H - 1);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        private IEnumerator DoEndOfTurnStuff(PhaseChangeAction pca)
        {
            // "At the end of the villain turn, {TheSimurghCharacter} deals the hero with the highest HP {H - 1} projectile damage.
            var e = DealDamageToHighestHP(
                CharacterCard,
                1,
                c => c.Is().Hero().Target(),
                c => H - 1,
                DamageType.Projectile
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            // Then, if there are no face-down villain cards, flip this card."
            if (FindCardsWhere(c => c.IsFaceDownNonCharacter && c.Is().Villain().AccordingTo(this)).Count() > 0)
            {
                e = FlipThisCharacterCardResponse(pca);
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

        private IEnumerator StackDeck(int numberOfCards)
        {
            var revealedCards = new List<Card>();
            var e = GameController.RevealCards(
                TurnTakerController,
                TurnTaker.Deck,
                numberOfCards,
                revealedCards,
                revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards,
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

            revealedCards.Sort(CompareSimurghDanger);
            revealedCards.Reverse();
            e = GameController.BulkMoveCards(TurnTakerController, revealedCards, TurnTaker.Deck, responsibleTurnTaker: TurnTaker, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        private int CompareSimurghDanger(Card lhs, Card rhs)
        {
            var l = lhs as ISimurghDangerCard;
            var r = rhs as ISimurghDangerCard;

            if (l == null && r == null) { return 0; }
            if (l == null) { return 1; }
            if (r == null) { return -1; }

            return l.Danger() - r.Danger();
        }
    }
}
