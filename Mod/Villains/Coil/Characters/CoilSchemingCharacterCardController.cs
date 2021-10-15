using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Coil
{
    public class CoilSchemingCharacterCardController : SubCoilCharacterCardController
    {
        public CoilSchemingCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        protected override void AddCoilTriggers()
        {
            //"If Scheming's HP is greater than or equal to Acting's HP, use {magic} text.",

            //"{magic}: At the start of the villain turn play the top card of the environment deck.",
            AddSideTrigger(AddStartOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => MaybePlayEnvironment(pca),
                TriggerType.PlayCard
            ));

            //"At the end of the villain turn all villain targets regain 2 HP",
            AddSideTrigger(AddEndOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => HealVillains(),
                TriggerType.GainHP
            ));
        }

        private IEnumerator MaybePlayEnvironment(PhaseChangeAction pca)
        {
            if (! UseExtraText()) { yield break; }

            var e = PlayTheTopCardOfTheEnvironmentDeckResponse(pca);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        private IEnumerator HealVillains()
        {
            return GameController.GainHP(
                DecisionMaker,
                c => c.Is(this).Villain().Target(),
                2,
                cardSource: GetCardSource()
            );
        }
    }
}
