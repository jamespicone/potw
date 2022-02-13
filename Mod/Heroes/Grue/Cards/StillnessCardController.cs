using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Grue
{
    public class StillnessCardController : CardController
    {
        public StillnessCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // Reduce all fire, sonic, lightning, and energy damage that would be dealt to or by a target with a Darkness card next to it by 1
            //
            // This is one trigger that fires once even if target and source both have darkness adjacent deliberately; that's my intended interpretation
            // of the card.
            AddReduceDamageTrigger(
                dda => 
                    (this.DoesTargetHaveDarknessAdjacent(dda.Target) || this.DoesTargetHaveDarknessAdjacent(dda.DamageSource.Card)) &&
                    (
                        dda.DamageType == DamageType.Fire ||
                        dda.DamageType == DamageType.Sonic ||
                        dda.DamageType == DamageType.Lightning ||
                        dda.DamageType == DamageType.Energy
                    ),
                dda => 1);

            AddStartOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => DarkenSomeone(),
                TriggerType.PutIntoPlay
            );
        }

        private IEnumerator DarkenSomeone()
        {
            var selectedTarget = new List<SelectCardDecision>();
            var e = GameController.SelectCardAndStoreResults(
                HeroTurnTakerController,
                SelectionType.MoveCardNextToCard,
                new LinqCardCriteria(c => c.IsTarget && c.IsInPlay, "target"),
                storedResults: selectedTarget,
                optional: false,
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

            var target = GetSelectedCard(selectedTarget);
            if (target == null) { yield break; }

            e = this.PutDarknessIntoPlay(target);
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
