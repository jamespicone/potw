using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

using UnityEngine;

namespace Jp.ParahumansOfTheWormverse.TheSimurgh
{
    public class TheSimurghCharacterCardController : VillainCharacterCardController
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
            AddDefeatedIfDestroyedTriggers();
            AddDefeatedIfMovedOutOfGameTriggers();

            if (Card.IsFlipped)
            {
                // Reduce damage dealt to {TheSimurghCharacter} by 2.
                AddSideTrigger(AddReduceDamageTrigger(c => c == Card, 2));

                // At the start of the villain turn, reveal the top {H + 1} cards of the villain deck. Put the revealed cards back in ascending order of {SimurghDanger}.
                AddSideTrigger(AddStartOfTurnTrigger(tt => tt == TurnTaker, pca => TurnTakerController.StackDeck(H + 1, GetCardSource()), TriggerType.RevealCard));

                AddSideTrigger(AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => DoFlippedEndOfTurnStuff(pca), new TriggerType[] { TriggerType.DealDamage, TriggerType.AddTokensToPool }));

                if (IsGameAdvanced)
                {
                    AddSideTrigger(AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => GetATrap(pca), new TriggerType[] { TriggerType.PutIntoPlay }));
                }
            }
            else
            {
                // Reduce damage dealt to {TheSimurghCharacter} by 1
                AddSideTrigger(AddReduceDamageTrigger(c => c == Card, 1));
                AddSideTrigger(AddStartOfTurnTrigger(tt => tt == TurnTaker, pca => DoStartOfTurnStuff(), new TriggerType[] { TriggerType.PlayCard, TriggerType.RevealCard }));
                AddSideTrigger(AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => DoEndOfTurnStuff(pca), new TriggerType[] { TriggerType.DealDamage, TriggerType.FlipCard }));

                if (IsGameAdvanced)
                {
                    // At the start of the villain turn, play the top card of the Condition deck.
                    AddSideTrigger(AddStartOfTurnTrigger(
                        tt => tt == TurnTaker,
                        pca => GameController.PlayTopCardOfLocation(TurnTakerController, ConditionDeck, responsibleTurnTaker: TurnTaker, cardSource: GetCardSource()),
                        TriggerType.PlayCard
                    ));
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
            e = TurnTakerController.StackDeck(H - 1, GetCardSource());
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
            if (FindCardsWhere(c => c.IsFaceDownNonCharacter && c.Is().Villain().AccordingTo(this)).Count() <= 0)
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

        private IEnumerator DoFlippedEndOfTurnStuff(PhaseChangeAction pca)
        {
            // At the end of the villain turn, put a Scream token on this card...
            var pool = Card.FindTokenPool("ScreamPool");
            if (pool == null) { yield break; }

            var e = GameController.AddTokensToPool(pool, 1, GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            // Then, this card deals each hero target X psychic damage, where X is the number of Scream tokens on this card.
            e = DealDamage(
                Card,
                c => c.Is().Hero().Target(),
                c => pool.CurrentValue,
                DamageType.Psychic
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

        private IEnumerator GetATrap(PhaseChangeAction pca)
        {
            var storedResults = new List<SelectCardDecision>();
            var e = GameController.SelectCardAndStoreResults(
                DecisionMaker,
                SelectionType.PutIntoPlay,
                new LinqCardCriteria(c => c.DoKeywordsContain("trap") && c.Location.IsOffToTheSide, "Trap card off to the side", useCardsSuffix: false),
                storedResults,
                optional: false,
                gameAction: pca,
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

            var selectedCard = GetSelectedCard(storedResults);
            if (selectedCard == null) { yield break; }

            e = GameController.PlayCard(
                TurnTakerController,
                selectedCard,
                isPutIntoPlay: true,
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
