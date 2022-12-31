using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Skitter
{
    public class SabotageSwarmCardController : CardController
    {
        public SabotageSwarmCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            AddEndOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => MaybeDestroyACard(),
                new TriggerType[] { TriggerType.DestroyCard, TriggerType.ModifyTokens }
            );

            this.AddResetTokenTrigger();
        }

        private IEnumerator MaybeDestroyACard()
        {
            // At the end of your turn you may destroy an Ongoing or Environment card.
            var storedDestroy = new List<DestroyCardAction>();
            var e = GameController.SelectAndDestroyCard(
                HeroTurnTakerController,
                new LinqCardCriteria(c => c.Is().Environment().Card() || c.Is().WithKeyword("ongoing").AccordingTo(this)),
                optional: true,
                storedResultsAction: storedDestroy,
                responsibleCard: Card,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            if (DidDestroyCard(storedDestroy) && Card.IsInPlay)
            {
                // If you do, either remove 2 Bug tokens from this card or destroy it.
                var pool = Card.FindBugPool();
                bool canTakeTokens = pool != null && Card.BugTokenCount() >= 2;

                var choices = new List<Function>
                {
                    new Function(
                        HeroTurnTakerController,
                        "Remove 2 bug tokens",
                        SelectionType.RemoveTokens,
                        () => GameController.RemoveTokensFromPool(pool, 2, cardSource: GetCardSource()),
                        onlyDisplayIfTrue: canTakeTokens
                    ),

                    new Function(
                        HeroTurnTakerController,
                        "Destroy this card",
                        SelectionType.DestroySelf,
                        () => GameController.DestroyCard(HeroTurnTakerController, Card, cardSource: GetCardSource()),
                        forcedActionMessage: $"{Card.Title} does not have 2 tokens on it, so it must be destroyed."
                    )
                };

                e = SelectAndPerformFunction(
                    HeroTurnTakerController,
                    choices,
                    gameAction: storedDestroy.FirstOrDefault()
                );
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }
        }
    }
}
