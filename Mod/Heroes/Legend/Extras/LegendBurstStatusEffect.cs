using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Jp.ParahumansOfTheWormverse.Legend
{
    [Serializable]
    class LegendBurstStatusEffect : OnPhaseChangeStatusEffect
    {
        public LegendBurstStatusEffect(Card cardWithMethod, string nameOfMethod, string description, Card cardSource, TurnTaker affected)
            : base(cardWithMethod, nameOfMethod, description, new TriggerType[] { TriggerType.Hidden }, cardSource)
        {
            AffectedTurnTaker = affected;
        }

        public TurnTaker AffectedTurnTaker { get; private set; }
    }
}
