using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Behemoth
{
    [TestFixture()]
    public class UnstoppableAdvanceTests : BehemothTestBase
    {
        [Test()]
        public void TestEndOfTurnMovesTokenToHighest()
        {
            SetupBehemothGame();
            RemoveBehemothTriggers();
            ClearProximity();

            PlayCard("UnstoppableAdvance");

            SetProximity(legacy, 3);
            SetProximity(bunker, 1);
            SetProximity(haka, 0);

            // Legacy has the most tokens; Bunker is the only other hero with one to give.
            GoToEndOfTurn();

            Assert.That(Proximity(legacy).CurrentValue, Is.EqualTo(4));
            Assert.That(Proximity(bunker).CurrentValue, Is.EqualTo(0));
            Assert.That(Proximity(haka).CurrentValue, Is.EqualTo(0));
        }
    }
}
