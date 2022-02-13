using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Legend
{
    public class SkyHighCardController : CardController
    {
        public SkyHighCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            AddThisCardControllerToList(CardControllerListType.ChangesVisibility);
        }

        public override bool? AskIfCardIsVisibleToCardSource(Card card, CardSource cardSource)
        {
            if (card.Owner != TurnTaker) { return null; }
            if (! cardSource.Card.Is().Environment()) { return null; }

            return false;
        }
    }
}
