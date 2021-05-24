using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Slaughterhouse9
{
    public class SpiderbotsCardController : CardController
    {
        public SpiderbotsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // At the end of the villain turn this card deals X melee damage to the hero target with the lowest HP, where X = 1 + the number of Spiderbots in play
            AddDealDamageAtEndOfTurnTrigger(
                TurnTaker,
                Card,
                c => c.IsHero && c.IsTarget,
                TargetType.LowestHP,
                1,
                DamageType.Melee,
                dynamicAmount: c => 1 + SpiderbotCount()
            );
        }

        private int SpiderbotCount()
        {
            return FindCardsWhere(
                new LinqCardCriteria(c => c.Identifier == "Spiderbots" && c.IsInPlayAndHasGameText),
                GetCardSource()
            ).Count();
        }
    }
}
