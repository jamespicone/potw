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
    public class StormOfStingersCardController : CardController
    {
        public StormOfStingersCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // "You may move a Bug token from one of your cards to {SkitterCharacter} or a Strategy card.",
            var e = this.MoveBugTokens(false, isOptional: true);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            var pool = CharacterCard.FindBugPool();
            int tokensRemoved = 0;

            // Remove any number of Bug tokens from {SkitterCharacter}.
            if (pool != null) {
                var tokensRemovedList = new List<int?>();
                e = RemoveAnyNumberOfTokensFromTokenPool(pool, tokensRemovedList);
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }

                tokensRemoved = tokensRemovedList.FirstOrDefault() ?? 0;
            }

            // {SkitterCharacter} deals up to X targets 2 melee damage and 2 toxic damage, where X = 1 + the number of tokens you removed.
            var attacks = new List<DealDamageAction>();
            attacks.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), target: null, amount: 2, DamageType.Melee));
            attacks.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), target: null, amount: 2, DamageType.Toxic));

            e = SelectTargetsAndDealMultipleInstancesOfDamage(
                attacks,
                c => true,
                minNumberOfTargets: 0,
                maxNumberOfTargets: tokensRemoved + 1
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
