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
    public class ShiftTests : BehemothTestBase
    {
        [Test()]
        public void TestEachHeroPassesPoolAlong()
        {
            SetupBehemothGame();
            ClearProximity();
            SetProximity(legacy, 3);
            SetProximity(bunker, 1);
            SetProximity(haka, 0);

            PlayMovementCard("Shift");

            // Each hero receives the previous hero's tokens (in turn order).
            Assert.That(Proximity(legacy).CurrentValue, Is.EqualTo(0));
            Assert.That(Proximity(bunker).CurrentValue, Is.EqualTo(3));
            Assert.That(Proximity(haka).CurrentValue, Is.EqualTo(1));
        }
    }
}
