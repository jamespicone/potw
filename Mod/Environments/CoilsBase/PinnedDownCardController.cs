using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.CoilsBase
{
    public class PinnedDownCardController : CoilsBaseSelfDestructCardController
    {
        public PinnedDownCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "Players can't play Ongoing or Equipment cards."
            CannotPlayCards(cardCriteria: (Card c) => c.Location.IsHero && c.DoKeywordsContain(new string[]{ "ongoing", "equipment"}));
        }
    }
}
