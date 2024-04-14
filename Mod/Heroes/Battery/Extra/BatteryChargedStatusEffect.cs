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
    public class BatteryChargedStatusEffect : OnPhaseChangeStatusEffect
    {
        public BatteryChargedStatusEffect(Card cardWithMethod, string nameOfMethod, Card cardSource, Card chargedCard, bool temporary)
            : base(cardWithMethod, nameOfMethod, GetStatusEffectDescription(chargedCard, temporary), new TriggerType[] { TriggerType.Other }, cardSource)
        {
            TargetLeavesPlayExpiryCriteria.IsOneOfTheseCards = new List<Card> { chargedCard };
        }

        public Card ChargedCard { get { return TargetLeavesPlayExpiryCriteria.IsOneOfTheseCards.FirstOrDefault(); } }

        public override string ToString()
        {
            return Description;
        }

        public static string GetStatusEffectDescription(Card chargedCard, bool temporary)
        {
            string ret = $"{chargedCard.Title} is charged";
            if (temporary) ret += " until the start of their next turn.";
            return ret;
        }
    }
}