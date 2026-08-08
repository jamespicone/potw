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
    public class NoSafetyTests : BehemothTestBase
    {
        [Test()]
        public void TestFewestTokensHeroTakesOneFromEachOther()
        {
            SetupBehemothGame();
            ClearProximity();
            SetProximity(legacy, 3);
            SetProximity(bunker, 2);
            SetProximity(haka, 0);

            PlayMovementCard("NoSafety");

            Assert.That(Proximity(legacy).CurrentValue, Is.EqualTo(2));
            Assert.That(Proximity(bunker).CurrentValue, Is.EqualTo(1));
            Assert.That(Proximity(haka).CurrentValue, Is.EqualTo(2));
        }
    }
}
