using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.TheSimurgh
{
    public class AResponsePreparedCardController : SimurghPlayWhenRevealedCardController, ISimurghDangerCard
    {
        public AResponsePreparedCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        // "When this card is revealed, play it.",

        public int Danger()
        {
            return 4;
        }

        protected override string SurpriseMessage()
        {
            return "The Simurgh was prepared for you!";
        }

        public override IEnumerator Play()
        {
            // "Destroy all hero Ongoing cards."
            return GameController.DestroyCards(
                DecisionMaker,
                new LinqCardCriteria(c => c.Is().Hero() && c.IsOngoing, "hero ongoing"),
                cardSource: GetCardSource()
            );
        }
    }
}
