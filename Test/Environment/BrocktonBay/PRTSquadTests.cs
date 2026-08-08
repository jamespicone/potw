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
    public class PRTSquadTests : BrocktonBayTestBase
    {
        [Test()]
        public void TestEndOfTurnDamagesLowestTarget()
        {
            SetupBrocktonBayGame();

            var squad = PlayCard("PRTSquad");

            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 10);
            SetHitPoints(haka, 25);

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageType(DamageType.Projectile);
            AssertDamageSource(squad);

            GoToEndOfTurn(env);

            QuickHPCheck(0, -2, 0);
        }
    }
}
