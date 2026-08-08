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
    public class LeetTests : BrocktonBayTestBase
    {
        [Test()]
        public void TestEndOfTurnDamagesLowestTarget()
        {
            SetupBrocktonBayGame();

            var leet = PlayCard("Leet");

            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 10);
            SetHitPoints(haka, 25);

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageType(DamageType.Lightning);
            AssertDamageSource(leet);

            GoToEndOfTurn(env);

            QuickHPCheck(0, -2, 0);
        }

        [Test()]
        public void TestUberInPlayIncreasesDamage()
        {
            SetupBrocktonBayGame();

            PlayCard("Leet");
            PlayCard("Uber");

            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 10);
            SetHitPoints(haka, 25);

            QuickHPStorage(bunker);

            GoToEndOfTurn(env);

            // Leet deals 2 + 1 = 3 to the lowest target while Uber is in play.
            QuickHPCheck(-3);
        }
    }
}
