using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Echidna
{
    public class EchidnaBaseTest : BaseTest
    {
        protected TurnTakerController echidna { get { return FindVillain("Echidna"); } }

        public void ReturnAllTwisted()
        {
            var twisted = FindCardsWhere(c => c.DoKeywordsContain("twisted"));
            if (twisted.Count() <= 0) { return; }

            MoveCards(echidna, twisted, twisted.First().NativeDeck);
        }

        public void RemoveAllTwisted()
        {
            var twisted = FindCardsWhere(c => c.DoKeywordsContain("twisted"));
            if (twisted.Count() <= 0) { return; }

            MoveCards(echidna, twisted, twisted.First().Owner.OutOfGame);
        }
    }
}
