using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Environment.BrocktonBay
{
    [TestFixture()]
    public class UberTests : BrocktonBayTestBase
    {
        [Test()]
        public void TestEndOfTurnDamagesHighestTargetNoHealAlone()
        {
            SetupBrocktonBayGame();

            var uber = PlayCard("Uber");
            SetHitPoints(uber, 2);

            // Baron Blade (40 HP) is the highest non-environment target.
            QuickHPStorage(baron.CharacterCard, uber);

            GoToEndOfTurn(env);

            // 2 melee to Baron, and no healing without Leet.
            QuickHPCheck(-2, 0);
        }

        [Test()]
        public void TestUberAndLeetRegainHPTogether()
        {
            SetupBrocktonBayGame();

            var uber = PlayCard("Uber");
            var leet = PlayCard("Leet");
            SetHitPoints(uber, 2);
            SetHitPoints(leet, 2);

            GoToEndOfTurn(env);

            AssertHitPoints(uber, 3);
            AssertHitPoints(leet, 3);
        }
    }
}
