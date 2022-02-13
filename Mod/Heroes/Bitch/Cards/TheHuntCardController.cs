using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Bitch
{
    public class TheHuntCardController : CardController
    {
        public TheHuntCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("dog"), "dog"));
        }

        public override void AddTriggers()
        {
            // At the start of your turn each Dog in play may deal 1 melee damage to a target
            AddStartOfTurnTrigger(turntaker => turntaker == TurnTaker, action => Act(), TriggerType.DealDamage);
        }

        public System.Collections.IEnumerator Act()
        {
            return this.SelectTargetsToDealDamageToTarget(
                HeroTurnTakerController,
                c => c.DoKeywordsContain("dog"),
                c => true,
                1,
                DamageType.Melee
            );
        }
    }
}
