using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Armsmaster
{
    public class CombatPredictionArrayCardController : ModuleCardController
    {
        public CombatPredictionArrayCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator DoPrimary()
        {
            // Reveal the top 3 cards of the villain deck and put them back in any order
            yield break;
        }

        public override IEnumerator DoSecondary()
        {
            // Until the start of your next turn reduce damage dealt to Armsmaster by villain cards by 1
            ReduceDamageStatusEffect status = new ReduceDamageStatusEffect(1);
            status.SourceCriteria.IsVillain = true;
            status.TargetCriteria.IsSpecificCard = Card;
            status.UntilStartOfNextTurn(TurnTaker);

            var e = AddStatusEffect(status);
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
