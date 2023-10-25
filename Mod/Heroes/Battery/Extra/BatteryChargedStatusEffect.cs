using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra;
using Handelabra.Sentinels;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Jp.ParahumansOfTheWormverse.Battery
{
    [Serializable]
    public class BatteryChargedStatusEffect : ReflectionStatusEffect
    {
        public BatteryChargedStatusEffect(Card cardWithMethod, string nameOfMethod, Card cardSource, Card chargedCard)
            : base(cardWithMethod, nameOfMethod, $"{chargedCard.Title} is charged", new TriggerType[] { TriggerType.Other }, cardSource)
        {
            TargetLeavesPlayExpiryCriteria.IsOneOfTheseCards = new List<Card> { chargedCard };
        }

        public Card ChargedCard { get { return TargetLeavesPlayExpiryCriteria.IsOneOfTheseCards.FirstOrDefault(); } }

        public override string ToString()
        {
            return Description;
        }
    }
}