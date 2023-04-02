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
    public class CrushCardController : CardController
    {
        public CrushCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowListOfCardsInPlay(
                new LinqCardCriteria(c => c.Is(this).Hero().Target() && c.IsEngulfed(), "engulfed hero target")
            );
        }

        public override IEnumerator Play()
        {
            // {EchidnaCharacter} deals {H} melee damage to any hero target with an 'Engulfed' card next to them.
            return DealDamage(
                CharacterCard,
                c => c.Is(this).Hero().Target() && c.IsEngulfed(),
                c => H,
                DamageType.Melee
            );
        }
    }
}
