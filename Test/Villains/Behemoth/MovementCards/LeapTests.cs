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
    public class LeapTests : BehemothTestBase
    {
        [Test()]
        public void TestMostCardsHeroGainsTwoNeighboursOne()
        {
            SetupBehemothGame();
            ClearProximity();

            // Legacy has the most cards in play.
            PlayCard("DangerSense");
            PlayCard("InspiringPresence");

            PlayMovementCard("Leap");

            Assert.That(Proximity(legacy).CurrentValue, Is.EqualTo(2));
            Assert.That(Proximity(bunker).CurrentValue, Is.EqualTo(1));
            Assert.That(Proximity(haka).CurrentValue, Is.EqualTo(1));
        }
    }
}
