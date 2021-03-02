using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Bitch
{
    public class WhistleCardController : CardController
    {
        public WhistleCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            AddStartOfTurnTrigger(tt => tt == TurnTaker, action => DrawCard(HeroTurnTaker, optional: true), TriggerType.DrawCard);
            AddEndOfTurnTrigger(
                tt => tt == TurnTaker,
                action => SelectAndPlayCardFromHand(
                    HeroTurnTakerController,
                    cardCriteria: new LinqCardCriteria(card => card.DoKeywordsContain("order"))
                ),
                TriggerType.DrawCard
            );
        }
    }
}
