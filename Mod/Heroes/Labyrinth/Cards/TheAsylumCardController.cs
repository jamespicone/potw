using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Labyrinth
{
    public class TheAsylumCardController : ShapingCardController
    {
        public TheAsylumCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddShapingTriggers()
        {
            // At the end of {LabyrinthCharacter}'s turn this card deals 2 psychic damage to all villain targets.
            AddDealDamageAtEndOfTurnTrigger(
                TurnTaker,
                Card,
                c => c.Is().Villain().Target().AccordingTo(this),
                TargetType.All,
                2,
                DamageType.Psychic
            );

            // At the end of {LabyrinthCharacter}'s turn this card deals 3 irreducible psychic damage to {LabyrinthCharacter}.
            AddDealDamageAtEndOfTurnTrigger(
                TurnTaker,
                Card,
                c => c == CharacterCard,
                TargetType.All,
                3,
                DamageType.Psychic,
                isIrreducible: true
            );
        }
    }
}
