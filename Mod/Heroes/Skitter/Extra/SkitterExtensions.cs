using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Skitter
{
    public static class SkitterExtensions
    {
        public static TokenPool FindBugPool(this Card card)
        {
            return card.FindTokenPool("BugPool");
        }

        public static int BugTokenCount(this Card card)
        {
            var pool = card.FindBugPool();
            if (pool == null) { return 0; }

            return pool.CurrentValue;
        }

        public static IEnumerator AddBugTokenToSkitter(this CardController co, int tokensToAdd)
        {
            var pool = co.CharacterCard.FindBugPool();
            if (pool == null) { yield break; } // sorry guise

            var e = co.GameController.AddTokensToPool(pool, tokensToAdd, co.GetCardSource());
            if (co.UseUnityCoroutines) { yield return co.GameController.StartCoroutine(e); }
            else { co.GameController.ExhaustCoroutine(e); }
        }

        // Checks if a DDA is an environment turn taker source with a card source matching one of our cards. This isn't strictly speaking
        // the definition of "swarm damage" but I'm not sure how else we can tag the DDAs (maybe save a list of GUIDs?). We'll try this
        // for now because I think the places where it falls down would be very hard to find.
        //
        // Need to double-check how this ends up working out with Guise / AddAssociatedCardSource
        public static bool IsSwarmDamage(this CardController co, DealDamageAction dda)
        {
            return dda.DamageSource.Is().Environment().Noncard() && dda.CardSource?.TurnTakerController == co.TurnTakerController;
        }

        public static void AddResetTokenTrigger(this CardController co)
        {
            co.AddAfterLeavesPlayAction(() => { co.Card.FindBugPool()?.SetToInitialValue(); return co.GameController.DoNothing(); });
        }

        public static IEnumerator MoveBugTokens(this CardController co, bool moveArbitraryAmount, bool isOptional, List<bool> didMove = null)
        {
            IEnumerator e;

            didMove?.Add(false);

            Card bugSource = null;

            Func<Card, bool> possibleSource = c => c.IsInPlay && c.BugTokenCount() > 0 && c.Owner == co.TurnTaker;
            Func<Card, bool> possibleTarget = c => c.IsInPlay && c != bugSource && (c == co.CharacterCard || (c.Is().WithKeyword("strategy").AccordingTo(co) && c.Owner == co.TurnTaker));

            var possibleSources = co.GameController.FindCardsWhere(possibleSource, visibleToCard: co.GetCardSource());

            if (possibleSources.Count() <= 1)
            {
                bool noPossibleMoves = false;

                bugSource = possibleSources.FirstOrDefault();
                if (bugSource == null) { noPossibleMoves = true; }

                var possibleTargets = co.GameController.FindCardsWhere(possibleTarget, visibleToCard: co.GetCardSource());
                if (possibleTargets.Count() <= 0) { noPossibleMoves = true; }

                if (noPossibleMoves)
                {
                    e = co.GameController.SendMessageAction("There are no possible bug token moves; skipping", Priority.Medium, co.GetCardSource(), showCardSource: true);
                    if (co.UseUnityCoroutines) { yield return co.GameController.StartCoroutine(e); }
                    else { co.GameController.ExhaustCoroutine(e); }
                    yield break;
                }
            }

            var bugSourceList = new List<SelectCardDecision>();
            e = co.GameController.SelectCardAndStoreResults(
                co.HeroTurnTakerController,
                SelectionType.RemoveTokens,
                new LinqCardCriteria(possibleSource, useCardsSuffix: false, useCardsPrefix: true),
                bugSourceList,
                optional: isOptional,
                cardSource: co.GetCardSource()
            );
            if (co.UseUnityCoroutines) { yield return co.GameController.StartCoroutine(e); }
            else { co.GameController.ExhaustCoroutine(e); }

            bugSource = bugSourceList.FirstOrDefault()?.SelectedCard;
            if (bugSource == null) { yield break; }

            var bugTargetList = new List<SelectCardDecision>();
            e = co.GameController.SelectCardAndStoreResults(
                co.HeroTurnTakerController,
                SelectionType.AddTokens,
                new LinqCardCriteria(possibleTarget, $"{co.CharacterCard.Title} or a Strategy"),
                bugTargetList,
                optional: false,
                cardSource: co.GetCardSource()
            );
            if (co.UseUnityCoroutines) { yield return co.GameController.StartCoroutine(e); }
            else { co.GameController.ExhaustCoroutine(e); }

            var bugTarget = bugTargetList.FirstOrDefault()?.SelectedCard;
            if (bugTarget == null) { yield break; }

            var sourcePool = bugSource.FindBugPool();
            var targetPool = bugTarget.FindBugPool();

            if (sourcePool == null || targetPool == null) { yield break; }

            var tokensToMove = 1;
            if (moveArbitraryAmount)
            {
                if (sourcePool.CurrentValue <= 1)
                {
                    tokensToMove = sourcePool.CurrentValue;
                }
                else
                {
                    var toMoveList = new List<SelectNumberDecision>();
                    e = co.GameController.SelectNumber(
                        co.HeroTurnTakerController,
                        SelectionType.AddTokens,
                        1,
                        sourcePool.CurrentValue,
                        storedResults: toMoveList,
                        cardSource: co.GetCardSource()
                    );
                    if (co.UseUnityCoroutines) { yield return co.GameController.StartCoroutine(e); }
                    else { co.GameController.ExhaustCoroutine(e); }

                    tokensToMove = toMoveList.FirstOrDefault()?.SelectedNumber ?? 0;
                }
            }
            
            e = co.GameController.MoveTokensFromPoolToPool(sourcePool, targetPool, tokensToMove, co.GetCardSource());
            if (co.UseUnityCoroutines) { yield return co.GameController.StartCoroutine(e); }
            else { co.GameController.ExhaustCoroutine(e); }

            didMove?.Clear();
            didMove?.Add(true);
        }
    }
}
