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
    public class BuryInBugsCardController : CardController
    {
        public BuryInBugsCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // "You may move a Bug token from one of your cards to {SkitterCharacter} or a Strategy card.",
            var e = this.MoveBugTokens(false, isOptional: true);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // Remove any number of Bug tokens from {SkitterCharacter}. {SkitterCharacter} destroys up to X Ongoing cards, where X = 1 + the number of tokens you removed.
            var pool = CharacterCard.FindBugPool();
            var tokensRemoved = 0;

            if (pool != null)
            {
                var removedList = new List<int?>();
                e = RemoveAnyNumberOfTokensFromTokenPool(pool, removedList);
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }

                tokensRemoved = removedList.FirstOrDefault() ?? 0;
            }

            e = GameController.SelectAndDestroyCards(
                HeroTurnTakerController,
                new LinqCardCriteria(c => c.Is().WithKeyword("ongoing").AccordingTo(this), "ongoing"),
                tokensRemoved + 1,
                optional: false,
                requiredDecisions: 0,
                responsibleCard: CharacterCard,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            return SkitterExtensions.GetMoveBugTokensCustomDecisionText(decision);
        }

    }
}
