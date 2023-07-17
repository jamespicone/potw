
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.JessicaYamada
{
    class TimeToThinkCardController : CardController
    {
        public override int AskPriority => 100;
        private bool insideAskMethod = false;

        public TimeToThinkCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.ChangesPhaseOrder);
        }

        public override void AddTriggers()
        {
            /*
                "Whenever a player plays no cards during their play phase, they may draw a card",
                "Whenever a player uses no powers during their power phase, they may draw a card"
            */
            AddTrigger<PhaseChangeAction>(
                (p) =>
                    p.ToPhase.Phase == Phase.Unknown && p.ToPhase.PhaseActionCountUsed == 42,
                TurnIntoPlayPhase,
                new TriggerType[1] { TriggerType.ChangePhaseOrder },
                TriggerTiming.Before
            );

            AddPhaseChangeTrigger(
                tt => tt.Is(this).Hero(),
                p => true,
                pca => 
                    (pca.FromPhase.PhaseActionCountUsed ?? 0) == 0 &&
                    (
                        pca.FromPhase.Phase == Phase.PlayCard ||
                        pca.FromPhase.Phase == Phase.UsePower
                    ),
                pca => DrawCardResponse(pca),
                new[] { TriggerType.FirstTrigger },
                TriggerTiming.Before
            );
        }

        private IEnumerator TurnIntoPlayPhase(PhaseChangeAction pca)
        {
            var playPhase = pca.ToPhase.TurnTaker.TurnPhases.Where(p => p.IsPlayCard).FirstOrDefault();
            if (playPhase != null)
            {
                pca.ToPhase = playPhase;
            }
            yield break;
        }

        public override TurnPhase AskIfTurnPhaseShouldBeChanged(TurnPhase fromPhase, TurnPhase toPhase)
        {
            if (insideAskMethod) return null;
            insideAskMethod = true;
            var expectedPhase = GameController.FindNextTurnPhase(fromPhase);
            insideAskMethod = false;

            if (expectedPhase.IsHero && expectedPhase.IsPlayCard)
            {
                return new TurnPhase(toPhase.TurnTaker, Phase.Unknown)
                {
                    PhaseActionCountUsed = 42 // secret flag
                };
            }
            else
            {
                return expectedPhase;
            }
        }

        public IEnumerator DrawCardResponse(PhaseChangeAction pca)
        {
            var htt = pca.FromPhase.TurnTaker as HeroTurnTaker;
            if (htt == null) { yield break; }

            var e = GameController.DrawCard(htt, optional: true, cardSource: GetCardSource());
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
