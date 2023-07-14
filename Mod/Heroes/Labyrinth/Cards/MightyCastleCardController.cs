using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Labyrinth
{
    public class MightyCastleCardController : ShapingCardController
    {
        public MightyCastleCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddShapingTriggers()
        {
            // At the end of {LabyrinthCharacter}'s turn...
            AddEndOfTurnTrigger(tt => tt == TurnTaker, ReduceDamage, TriggerType.ReduceDamage);
        }

        private IEnumerator ReduceDamage(PhaseChangeAction pca)
        {
            // pick a target.
            var storedResults = new List<SelectCardDecision>();
            var e = GameController.SelectCardAndStoreResults(
                HeroTurnTakerController,
                SelectionType.ReduceDamageTaken,
                new LinqCardCriteria(c => c.Is().Target() && c.IsInPlay, "target", useCardsSuffix: false),
                storedResults,
                optional: false,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            var selectedTarget = GetSelectedCard(storedResults);
            if (selectedTarget == null) yield break;

            // Until the start of Labyrinth's next turn, reduce damage dealt to that target by 1.
            var effect = new ReduceDamageStatusEffect(1);
            effect.TargetCriteria.IsSpecificCard = selectedTarget;
            effect.UntilTargetLeavesPlay(selectedTarget);
            effect.UntilStartOfNextTurn(TurnTaker);

            e = AddStatusEffect(effect);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
