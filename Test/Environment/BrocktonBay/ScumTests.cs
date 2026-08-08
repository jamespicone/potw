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
    public class ScumTests : BrocktonBayTestBase
    {
        [Test()]
        public void TestDamagesSecondHighestTarget()
        {
            SetupBrocktonBayGame();

            var scum = PlayCard("Scum", 0);

            // Baron Blade (40) is the highest non-environment target; Haka is second.
            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 10);
            SetHitPoints(haka, 30);

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageType(DamageType.Projectile);
            AssertDamageSource(scum);

            GoToEndOfTurn(env);

            // X = 2 x 1 Scum in play.
            QuickHPCheck(0, 0, -2);
        }

        [Test()]
        public void TestDamageScalesWithScumInPlay()
        {
            SetupBrocktonBayGame();

            PlayCard("Scum", 0);
            PlayCard("Scum", 1);

            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 10);
            SetHitPoints(haka, 30);

            QuickHPStorage(haka);

            GoToEndOfTurn(env);

            // Each of the two Scum deals 2 x 2 = 4 to the second-highest target.
            QuickHPCheck(-8);
        }
    }
}
