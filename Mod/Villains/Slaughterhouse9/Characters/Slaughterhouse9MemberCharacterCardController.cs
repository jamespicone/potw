using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Slaughterhouse9
{
    public class Slaughterhouse9MemberCharacterCardController : VillainCharacterCardController
    {
        public Slaughterhouse9MemberCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        protected ITrigger AddAttackTrigger(Func<IEnumerator> response, IEnumerable<TriggerType> triggerTypes, string name)
        {
            return AddNineTrigger(response, triggerTypes, name, "attack");
        }

        protected ITrigger AddDefenceTrigger(Func<IEnumerator> response, IEnumerable<TriggerType> triggerTypes, string name)
        {
            return AddNineTrigger(response, triggerTypes, name, "defence");
        }

        protected ITrigger AddSpecialTrigger(Func<IEnumerator> response, IEnumerable<TriggerType> triggerTypes, string name)
        {
            return AddNineTrigger(response, triggerTypes, name, "special");
        }

        private ITrigger AddNineTrigger(Func<IEnumerator> response, IEnumerable<TriggerType> triggerTypes, string name, string keyword)
        {
            return AddTrigger<MoveCardAction>(
                mca => mca.Destination == TurnTaker.Trash && mca.CardToMove.DoKeywordsContain(keyword),
                mca => DoCardResponse(mca, response, name),
                triggerTypes,
                TriggerTiming.After
            );
        }

        private IEnumerator DoCardResponse(MoveCardAction mca, Func<IEnumerator> response, string name)
        {
            if (HasBeenSetToTrueThisTurn(name))
            {
                yield break;
            }

            SetCardPropertyToTrueIfRealAction(name, gameAction: mca);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(response());
            }
            else
            {
                GameController.ExhaustCoroutine(response());
            }
        }
    }
}
