using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Bitch
{
    public class BaseDogCardController : CardController
    {
        public BaseDogCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override System.Collections.IEnumerator Play()
        {
            yield break;
        }

        public override void AddTriggers()
        {
            this.AddDealDamageAtEndOfTurnTrigger(this.TurnTaker, this.Card, card => card == this.Card, TargetType.SelectTarget, 1, DamageType.Psychic);
        }
    }
}
