
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.JessicaYamada
{
    class SupportAndStabilityCardController : CardController
    {
        public SupportAndStabilityCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            AllowFastCoroutinesDuringPretend = false;
        }

        public override void AddTriggers()
        {
            // "The first time each turn a hero would deal themselves damage, you may prevent that damage"
            AddTrigger<DealDamageAction>(
                dda => this.HasAlignmentCharacter(dda.Target, CardAlignment.Hero, CardTarget.Target) && dda.Target == dda.DamageSource.Card,
                dda => MaybePrevent(dda),
                TriggerType.CancelAction,
                TriggerTiming.Before
            );
        }

        public IEnumerator MaybePrevent(DealDamageAction dda)
        {
            // Check if it's the first time this turn a hero has dealt themselves damage
            if (HasBeenSetToTrueThisTurn("JessicaSupportStability"))
            {
                yield break;
            }

            SetCardPropertyToTrueIfRealAction("JessicaSupportStability");

            if (GameController.PretendMode || preventDamage == null)
            {
                var storedYesNo = new List<YesNoCardDecision>();
                var e = GameController.MakeYesNoCardDecision(
                    HeroTurnTakerController,
                    SelectionType.PreventDamage,
                    Card,
                    storedResults: storedYesNo,
                    associatedCards: new Card[]
                    {
                    dda.Target
                    },
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

                preventDamage = DidPlayerAnswerYes(storedYesNo);
            }

            if (preventDamage.GetValueOrDefault(false))
            { 
                var e = CancelAction(dda, isPreventEffect: true);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }
            }

            if (! GameController.PretendMode)
            {
                preventDamage = null;
            }
        }

        private bool? preventDamage;
    }
}
