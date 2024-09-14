using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.TheMerchants
{
    public class ScrubCardController : CardController
    {
        public ScrubCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowListOfCardsInPlay(
                new LinqCardCriteria(
                    c => IsTargeted(c),
                    $"other than {Card.Title} with HP evenly divisible by 3",
                    useCardsSuffix: false,
                    useCardsPrefix: true
                )
            );
        }

        private bool IsTargeted(Card c)
        {
            return c.IsTarget && c.HitPoints.HasValue && c.HitPoints.Value % 3 == 0 && c != Card;
        }

        public override void AddTriggers()
        {
            // At the start of the villain turn, this card deals 5 energy damage
            // to each target other than itself whose HP is evenly divisible by 3.
            AddDealDamageAtStartOfTurnTrigger(
                TurnTaker,
                Card,
                c => IsTargeted(c),
                TargetType.All,
                5,
                DamageType.Energy
            );
        }
    }
}
