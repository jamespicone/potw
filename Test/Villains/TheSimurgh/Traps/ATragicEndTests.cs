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
    public class ATragicEndTests : SimurghTestBase
    {
        [Test()]
        public void TestFlipDestroysTargetsAndDealsDamage()
        {
            SetupSimurghGame();
            RemoveSimurghTriggers();

            var pileup = PlayCard("TrafficPileup");
            var monorail = PlayCard("PlummetingMonorail");

            QuickHPStorage(legacy, bunker, haka);

            FlipTrapWithPlanEnacted("ATragicEnd");

            AssertInTrash(pileup);
            AssertInTrash(monorail);

            // X = 2 targets destroyed: 2 projectile to each hero target.
            QuickHPCheck(-2, -2, -2);
        }
    }
}
