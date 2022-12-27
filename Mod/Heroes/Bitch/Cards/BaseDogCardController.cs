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

        public override void AddTriggers()
        {
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, card => card == Card, TargetType.SelectTarget, 1, DamageType.Psychic);
        }
    }
}
