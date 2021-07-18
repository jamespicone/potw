using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dauntless
{
    public static class DauntlessExtensions
    {
        public static bool HasPlasmaCore(this Card co)
        {
            return co.GetAllNextToCards(false).Where(c => c.Identifier == "PlasmaCore").Count() > 0;
        }

        public static bool HasMatterToEnergy(this Card co)
        {
            return co.GetAllNextToCards(false).Where(c => c.Identifier == "MatterToEnergy").Count() > 0;
        }

        public static int ChargeCount(this CardController co)
        {
            return co.Card.GetAllNextToCards(false).Where(c => c.DoKeywordsContain("charge")).Count();
        }
    }
}
