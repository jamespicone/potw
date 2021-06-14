using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Jp.ParahumansOfTheWormverse.Dragon
{
    public class AnalysisCardController : CardController
    {
        public AnalysisCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            Debug.Log("Analysis::Play");

            var effect = new IncreaseDamageStatusEffect(1);
            effect.SourceCriteria.HasAnyOfTheseKeywords = new List<string>();
            effect.SourceCriteria.HasAnyOfTheseKeywords.Add("mech");
            effect.SourceCriteria.IsPlayAreaOf = TurnTaker;
            effect.UntilThisTurnIsOver(Game);

            Debug.Log("Analysis::Play adding status effect");
            var e = GameController.AddStatusEffect(effect, true, GetCardSource());
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

            Debug.Log("Analysis::Play adding status effect 2");
            e = GameController.AddStatusEffect(effect, true, GetCardSource());
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
