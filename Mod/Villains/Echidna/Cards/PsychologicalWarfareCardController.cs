using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Echidna
{
    public class PsychologicalWarfareCardController : CardController
    {
        public PsychologicalWarfareCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // At the start of the villain turn {EchidnaCharacter} deals 2 psychic damage to all hero targets.
            AddDealDamageAtStartOfTurnTrigger(
                TurnTaker,
                CharacterCard,
                c => c.Is(this).Hero().Target(),
                TargetType.All,
                2,
                DamageType.Psychic
            );
        }
    }
}
