using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Legend
{
    public class BurstCardController : CardController
    {
        public BurstCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            var effect = new LegendBurstStatusEffect(Card, nameof(DoNothing), $"This turn, when {HeroTurnTaker.NameRespectingVariant} chooses an effect, they may choose any number of effects", Card, TurnTaker);
            effect.UntilThisTurnIsOver(Game);

            var e = AddStatusEffect(effect);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        public IEnumerator DoNothing(PhaseChangeAction pca, OnPhaseChangeStatusEffect sourceEffect)
        {
            yield break;
        }
    }
}
