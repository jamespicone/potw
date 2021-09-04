using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra;

namespace Jp.ParahumansOfTheWormverse.Legend
{

    public class LegendCharacterCardController : HeroCharacterCardController
    {
        public LegendCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        public override void AddTriggers()
        {
            AddTrigger<DealDamageAction>(
                dda => true,
                dda => PrintDDA(dda),
                TriggerType.FirstTrigger,
                TriggerTiming.Before
            );            
        }

        IEnumerator PrintDDA(DealDamageAction dda)
        {
            Log.Debug(LogName.ResolvedAction, $"DebugDDA: {dda.SourceString} damages {dda.Target.Title}. Preview: {GameController.PreviewMode}. Pretend: {GameController.PretendMode}. DDA Pretend: {dda.IsPretend}");
            yield break;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            yield break;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // You may play a card.
            var e = GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, Card),
                1, DamageType.Radiant,
                numberOfTargets: 1,
                optional: false,
                requiredTargets: 1,
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
    }
}
