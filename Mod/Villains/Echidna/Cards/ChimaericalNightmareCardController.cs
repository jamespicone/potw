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
    public class ChimaericalNightmareCardController : CardController
    {
        public ChimaericalNightmareCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // Whenever an environment target is destroyed play the top card of the villain deck.
            AddTrigger<DestroyCardAction>(
                dca => dca.WasCardDestroyed && dca.CardToDestroy.Is().Environment().Target(),
                dca => PlayTheTopCardOfTheVillainDeckResponse(dca),
                TriggerType.PlayCard,
                TriggerTiming.After
            );
        }
    }
}
