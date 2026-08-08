using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.TheSimurgh
{
    [TestFixture()]
    public class ACapabilityRevealedTests : SimurghTestBase
    {
        [Test()]
        public void TestDestroysAllEquipment()
        {
            SetupSimurghGame();
            RemoveSimurghTriggers();

            var flak = PlayCard("FlakCannon");
            var gatling = PlayCard("GatlingGun");

            PlayCard("ACapabilityRevealed");

            AssertInTrash(flak);
            AssertInTrash(gatling);
        }
    }
}
