using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Lung
{
    public class WhiteHotFlameCardController : CardController
    {
        public WhiteHotFlameCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }
        public override void AddTriggers()
        {
            // Fire damage dealt by {Lung} is irreducible
            // TODO: Should I be checking identifiers?
            AddMakeDamageIrreducibleTrigger(dda => dda.DamageType == DamageType.Fire && dda.DamageSource.Card == TurnTaker.CharacterCard);
        }
    }
}
