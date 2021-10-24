using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.TheSimurgh
{
    public class AFateSelectedCardController : CardController, ISimurghDangerCard
    {
        public AFateSelectedCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public int Danger()
        {
            // TODO
            return 0;
        }

        public override IEnumerator Play()
        {
            // {TheSimurghCharacter} deals the hero with the fewest cards in play {H} sonic damage.
            return DealDamageToMostCardsInPlay(
                CharacterCard,
                1,
                new LinqCardCriteria(c => c.Is().Hero().Character().Target(), "hero"),
                H,
                DamageType.Sonic,
                mostFewestSelectionType: SelectionType.FewestCardsInPlay
            );
        }
    }
}
