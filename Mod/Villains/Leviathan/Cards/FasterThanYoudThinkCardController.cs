using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Leviathan
{
    public class FasterThanYoudThinkCardController : CardController
    {
        public FasterThanYoudThinkCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            // This card is indestructible
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override void AddTriggers()
        {
            // The first time {LeviathanCharacter} would be dealt damage each round, prevent that damage.
            AddTrigger<DealDamageAction>(
                (dda) => dda.Amount > 0 && dda.Target == CharacterCard && ! AlreadyUsed(),
                PreventDamage,
                TriggerType.WouldBeDealtDamage,
                TriggerTiming.Before
            );
        }

        private bool AlreadyUsed()
        {
            var props = Journal.CardPropertiesEntriesThisRound(cp => cp.Card == Card && cp.Key == "LeviathanDamagedThisRound");
            return props.Where(cp => cp.BoolValue.HasValue && cp.BoolValue.Value).Count() > 0;
        }

        private IEnumerator PreventDamage(DealDamageAction dda)
        {
            SetCardPropertyToTrueIfRealAction("LeviathanDamagedThisRound");
            var e = CancelAction(
                dda,
                isPreventEffect: true
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
        
        public override bool AskIfCardIsIndestructible(Card card)
        {
            return card == Card;
        }

    }
}
