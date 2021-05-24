
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.JessicaYamada
{
    class SupportAndStabilityCardController : CardController
    {
        public SupportAndStabilityCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // "The first time each turn a hero would deal themselves damage, you may prevent that damage"
            AddTrigger<DealDamageAction>(
                dda => dda.Target.IsHeroCharacterCard && dda.Target == dda.DamageSource.Card,
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

            if (dda.IsPretend)
            {
                var e2 = CancelAction(dda, isPreventEffect: true);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e2);
                }
                else
                {
                    GameController.ExhaustCoroutine(e2);
                }

                yield break;
            }

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

            if (DidPlayerAnswerYes(storedYesNo))
            {
                e = CancelAction(dda, isPreventEffect: true);
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
}
