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
    public class ABackupSabotagedCardController : ConditionCardController
    {
        public ABackupSabotagedCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        protected override bool IsConditionMet()
        {
            // If any hero has 2 cards in hand with the same name
            var heroes = FindActiveHeroTurnTakerControllers();
            foreach (var httc in heroes)
            {
                // For each hero, if the number of unique card titles in their hand != the number of titles in their hand,
                // then there's at least one pair of cards with the same title.
                var cardTitles = httc.HeroTurnTaker.Hand.Cards.Select(c => c.AlternateTitleOrTitle);
                var uniqueCardTitles = cardTitles.Distinct();
                if (cardTitles.Count() != uniqueCardTitles.Count())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
