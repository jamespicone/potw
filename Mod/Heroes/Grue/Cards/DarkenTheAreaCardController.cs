using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Grue
{
    public class DarkenTheAreaCardController : CardController
    {
        public DarkenTheAreaCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // "Until the start of your next turn whenever an environment target enters play put a Darkness card next to it",
            AddTargetEntersPlayTrigger(
                c => c.Is().Environment().Target() && StatusEffectIsActive(),
                c => this.PutDarknessIntoPlay(c),
                TriggerType.PutIntoPlay,
                TriggerTiming.After,
                outOfPlayTrigger: true
            );
        }

        private bool StatusEffectIsActive()
        {
            return GameController.StatusEffectControllers.Count(sec => sec.StatusEffect.CardSource == Card) > 0;
        }

        public override IEnumerator Play()
        {
            var effect = new OnPhaseChangeStatusEffect(
                CardWithoutReplacements,
                nameof(DoNothingStatusEffect),
                $"Until the start of {HeroTurnTaker.NameRespectingVariant}'s next turn whenever an environment target enters play put a Darkness card next to it",
                new TriggerType[] { TriggerType.PutIntoPlay },
                Card
            );

            effect.UntilStartOfNextTurn(TurnTaker);

            var e = AddStatusEffect(effect);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            // "You may destroy an environment card"
            e = GameController.SelectAndDestroyCard(
                HeroTurnTakerController,
                new LinqCardCriteria(c => c.IsEnvironment, "environment"),
                optional: true,
                responsibleCard: Card,
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

        public IEnumerator DoNothingStatusEffect(PhaseChangeAction unused, OnPhaseChangeStatusEffect sourceEffect)
        {
            return DoNothing();
        }
    }
}
