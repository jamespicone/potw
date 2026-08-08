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
    public class AShiftOfAttentionTests : BehemothTestBase
    {
        [Test()]
        public void TestMostTokensPassedTwoTurnsBack()
        {
            SetupBehemothGame();
            ClearProximity();
            SetProximity(legacy, 4);
            SetProximity(bunker, 1);
            SetProximity(haka, 0);

            PlayMovementCard("AShiftOfAttention");

            // Legacy had the most tokens; two active heroes before him in turn order
            // (wrapping) is Bunker, who receives all 4.
            Assert.That(Proximity(legacy).CurrentValue, Is.EqualTo(0));
            Assert.That(Proximity(bunker).CurrentValue, Is.EqualTo(5));
            Assert.That(Proximity(haka).CurrentValue, Is.EqualTo(0));
        }
    }
}
