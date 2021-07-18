using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Alexandria
{
    public class ProstheticEyeCardController : CardController
    {
        public ProstheticEyeCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // "At the end of your turn reveal the top 3 cards of a deck. Put them back in any order"
        }
    }
}
