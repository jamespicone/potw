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
    public class BugClonesCardController : CardController
    {
        public BugClonesCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            AllowFastCoroutinesDuringPretend = false;
        }

        public override void AddTriggers()
        {
            // Whenever {SkitterCharacter} would be dealt damage you may prevent it.
            AddTrigger<DealDamageAction>(
                dda => dda.Target == CharacterCard,
                dda => MaybePreventDamage(dda),
                new TriggerType[] { TriggerType.ImmuneToDamage, TriggerType.ModifyTokens },
                TriggerTiming.Before
            );

            this.AddResetTokenTrigger();
        }

        public IEnumerator MaybePreventDamage(DealDamageAction dda)
        {
            if (! preventInfo.HasValue || preventInfo != dda.InstanceIdentifier)
            {
                var preventResult = new List<YesNoCardDecision>();
                var e = GameController.MakeYesNoCardDecision(
                    HeroTurnTakerController,
                    SelectionType.PreventDamage,
                    Card,
                    dda,
                    preventResult,
                    cardSource: GetCardSource()
                );

                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }

                if (DidPlayerAnswerYes(preventResult))
                {
                    preventInfo = dda.InstanceIdentifier;
                }
                else
                {
                    preventInfo = null;
                }
            }

            if (preventInfo.HasValue)
            {
                var e = GameController.CancelAction(dda, isPreventEffect: true, cardSource: GetCardSource());
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }

                if (! dda.IsPretend)
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
                        choices
                    );
                    if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                    else { GameController.ExhaustCoroutine(e); }
                }
            }

            if (! GameController.PretendMode)
            {
                preventInfo = null;
            }
        }

        private Guid? preventInfo;
    }
}