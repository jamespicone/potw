using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dragon
{
    public abstract class MechCardController : CardController
    {
        public MechCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        protected abstract int FocusCost();

        protected virtual void AddExtraTriggers() { }

        public override void AddTriggers()
        {
            AddStartOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => CharacterCardController.LoseFocusTokens(FocusCost(), GetCardSource()),
                TriggerType.ModifyTokens
            );

            AddExtraTriggers();
        }
    }
}
