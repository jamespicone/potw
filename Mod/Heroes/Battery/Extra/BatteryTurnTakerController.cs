using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Battery
{
    public class BatteryTurnTakerController : HeroTurnTakerController
    {
        public const string ChargedIdentifier = "ChargedIndicator";

        public BatteryTurnTakerController(TurnTaker tt, GameController gc):base(tt,gc)
        {

        }

        public void SetAsideIndicators()
        {
            IEnumerable<Card> indicatorsInDeckOrHand = TurnTaker.GetAllCards(false).Where((Card c) => !c.IsRealCard && c.Identifier == ChargedIdentifier && (c.Location == TurnTaker.Deck || c.Location == HeroTurnTaker.Hand));
            if (indicatorsInDeckOrHand.Any())
            {
                foreach(Card indicator in indicatorsInDeckOrHand)
                {
                    TurnTaker.MoveCard(indicator, TurnTaker.OffToTheSide);
                }
                while (HeroTurnTaker.NumberOfCardsInHand < 4)
                {
                    HeroTurnTaker.DrawCard(null);
                }
            }
        }
    }
}
