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
    public class AdvanceTests : BehemothTestBase
    {
        [Test()]
        public void TestEachHeroGainsToken()
        {
            SetupBehemothGame();
            ClearProximity();
            SetProximity(bunker, 1);
            SetProximity(haka, 2);

            var advance = PlayMovementCard("Advance");

            Assert.That(Proximity(legacy).CurrentValue, Is.EqualTo(1));
            Assert.That(Proximity(bunker).CurrentValue, Is.EqualTo(2));
            Assert.That(Proximity(haka).CurrentValue, Is.EqualTo(3));

            // Used movement cards go under the Movement Trash card.
            Assert.That(advance.Location, Is.EqualTo(MovementTrashPile));
        }
    }
}
