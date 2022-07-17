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
    public class ResistanceTwistedCardController : CardController
    {
        public ResistanceTwistedCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowHeroTargetWithLowestHP();
        }

        public override void AddTriggers()
        {
            // "At the start of the villain turn play the top card of the villain deck.",
            AddStartOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => PlayTheTopCardOfTheVillainDeckResponse(pca),
                TriggerType.PlayCard
            );

            // "At the end of the villain turn this card deals 2 melee damage to the hero target with the lowest HP."
            AddDealDamageAtEndOfTurnTrigger(
                TurnTaker,
                Card,
                c => c.Is().Hero().Target(),
                TargetType.LowestHP,
                2,
                DamageType.Melee
            );
        }
    }
}
