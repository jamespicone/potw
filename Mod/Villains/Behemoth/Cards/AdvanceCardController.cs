using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Behemoth
{
    public class AdvanceCardController : BehemothUtilityCardController
    {
        public AdvanceCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            // "Each player with an active hero puts a proximity token on their hero."
            IEnumerator addCoroutine = base.GameController.SelectTurnTakersAndDoAction(DecisionMaker, new LinqTurnTakerCriteria((TurnTaker tt) => tt.IsHero && !tt.IsIncapacitatedOrOutOfGame), SelectionType.AddTokens, (TurnTaker tt) => base.GameController.AddTokensToPool(ProximityPool(tt), 1, GetCardSource()), associatedCards: base.Card.ToEnumerable(), cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(addCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(addCoroutine);
            }
            yield break;
        }
    }
}
