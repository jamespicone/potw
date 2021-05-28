using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dragon
{
    public class AnalysisCardController : CardController
    {
        public AnalysisCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            var effect = new IncreaseDamageStatusEffect(1);
            effect.SourceCriteria.HasAnyOfTheseKeywords.Add("mech");
            effect.SourceCriteria.IsPlayAreaOf = TurnTaker;
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

            effect = new IncreaseDamageStatusEffect(1);
            effect.SourceCriteria.IsSpecificCard = CharacterCard;
            effect.UntilThisTurnIsOver(Game);

            e = AddStatusEffect(effect);
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
