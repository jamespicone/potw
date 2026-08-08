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
    public class FocusTests : BehemothTestBase
    {
        [Test()]
        public void TestHighestHPHeroGainsToken()
        {
            SetupBehemothGame();
            ClearProximity();

            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 20);
            SetHitPoints(haka, 30);

            PlayMovementCard("Focus");

            Assert.That(Proximity(legacy).CurrentValue, Is.EqualTo(0));
            Assert.That(Proximity(bunker).CurrentValue, Is.EqualTo(0));
            Assert.That(Proximity(haka).CurrentValue, Is.EqualTo(1));
        }
    }
}
