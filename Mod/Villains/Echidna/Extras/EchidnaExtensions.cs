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
    public static class EchidnaExtensions
    {
        public static bool IsAnEngulfedCard(this Card c)
        {
            return c.Identifier == "Engulfed" && c.ModNamespace == "Jp.ParahumansOfTheWormverse";
        }

        public static bool IsEngulfed(this Card c)
        {
            return c.GetAllNextToCards(recursive: false).Any(c2 => c2.IsAnEngulfedCard());
        }
    }
}
