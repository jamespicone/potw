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
    public class BatteryUtilityCharacterCardController : HeroCharacterCardController
    {
        public int DischargePowerIndex
        {
            get;
            protected set;
        }

        public BatteryUtilityCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.ShowChargedStatus(SpecialStringMaker, Card);
        }
    }
}
