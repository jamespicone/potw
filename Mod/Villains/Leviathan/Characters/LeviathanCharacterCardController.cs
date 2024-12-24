using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Leviathan
{
    public class LeviathanCharacterCardController : VillainCharacterCardController
    {
        public LeviathanCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddSideTriggers()
        {
            AddDefeatedIfDestroyedTriggers();
            AddDefeatedIfMovedOutOfGameTriggers();

            if (Card.IsFlipped)
            {
                // At the start of the villain turn, flip {Leviathan}
                AddSideTrigger(AddStartOfTurnTrigger(
                    tt => tt == TurnTaker,
                    pca => FlipThisCharacterCardResponse(pca),
                    TriggerType.FlipCard
                ));

                // Reduce damage dealt to Leviathan by 1
                AddSideTrigger(AddReduceDamageTrigger(
                    c => c == CharacterCard,
                    1
                ));

                if (IsGameAdvanced)
                {
                    AddSideTrigger(AddEndOfTurnTrigger(
                        tt => tt == TurnTaker,
                        PlayTheTopCardOfTheVillainDeckResponse,
                        TriggerType.PlayCard
                    ));
                }
            }
            else
            {
                // At the end of the villain turn, {Leviathan} deals 2 melee damage to all hero targets.
                AddSideTrigger(AddDealDamageAtEndOfTurnTrigger(
                    TurnTaker,
                    Card,
                    c => c.Is(this).Hero().Target() && c.IsInPlay,
                    TargetType.All,
                    amount: 2,
                    DamageType.Melee
                ));

                // When Leviathan is dealt {8 - H} or more damage put a Retaliation token on Leviathan.
                AddSideTrigger(AddTrigger<DealDamageAction>(
                    dda => dda.Target == CharacterCard && dda.DidDealDamage && dda.FinalAmount >= 8 - H,
                    dda => GainRetaliationToken(dda),
                    TriggerType.AddTokensToPool,
                    TriggerTiming.After
                ));

                // At the end of the villain turn, if {Leviathan} has any Retaliation tokens remove all of them then flip Leviathan.
                AddSideTrigger(AddEndOfTurnTrigger(
                    tt => tt == TurnTaker,
                    pca => MaybeFlip(pca),
                    new TriggerType[] { TriggerType.FlipCard, TriggerType.ModifyTokens }
                ));
            }
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            var e = base.AfterFlipCardImmediateResponse();
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            if (Card.IsFlipped)
            {
                // When flipped to this side, reveal cards from the top of the villain deck until you reveal a Tactic card.
                // Put it into play; shuffle the rest of the revealed cards back into the villain deck.
                e = RevealCards_MoveMatching_ReturnNonMatchingCards(
                    TurnTakerController,
                    TurnTaker.Deck,
                    playMatchingCards: false,
                    putMatchingCardsIntoPlay: true,
                    moveMatchingCardsToHand: false,
                    new LinqCardCriteria(c => c.DoKeywordsContain("tactic"), "Tactic card"),
                    numberOfMatches: 1,
                    revealedCardDisplay: RevealedCardDisplay.ShowMatchingCards
                );
            }
            else
            {
                // When flipped to this side, destroy {H} noncharacter hero cards.
                e = GameController.SelectAndDestroyCards(
                    DecisionMaker,
                    new LinqCardCriteria(c => c.Is(this).Hero().Noncharacter() && c.IsInPlay),
                    numberOfCards: H,
                    responsibleCard: Card,
                    cardSource: GetCardSource()
                );
            }

            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        private IEnumerator GainRetaliationToken(DealDamageAction dda)
        {
            var pool = Card.FindTokenPool("RetaliationPool");
            if (pool == null) { yield break; }

            IEnumerator e;
            if (pool.CurrentValue <= 0)
            {
                e = GameController.SendMessageAction(
                    $"{CharacterCard.Title} was dealt {dda.FinalAmount} damage and will retaliate!",
                    Priority.Medium,
                    GetCardSource(),
                    showCardSource: true
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

            e = GameController.AddTokensToPool(pool, 1, GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        private IEnumerator MaybeFlip(PhaseChangeAction pca)
        {
            var pool = Card.FindTokenPool("RetaliationPool");
            if (pool == null) { yield break; }

            if (pool.CurrentValue <= 0) { yield break; }

            var e = GameController.RemoveTokensFromPool(pool, pool.CurrentValue, gameAction: pca, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

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
}
