using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Environment.CoilsBase
{
    [TestFixture()]
    public class BlastDoorsTests : CoilsBaseTestBase
    {
        [Test()]
        public void TestReductionGrowsWithDamageThisTurn()
        {
            SetupCoilsBaseGame();

            PlayCard("BlastDoors");

            QuickHPStorage(bunker);

            // No damage dealt yet this turn: X = 0.
            DealDamage(haka, bunker, 3, DamageType.Fire);
            QuickHPCheck(-3);

            // One damage dealt this turn: X = 1.
            DealDamage(haka, bunker, 3, DamageType.Fire);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestDamagesItselfAtStartOfEnvironmentTurn()
        {
            SetupCoilsBaseGame();

            var doors = PlayCard("BlastDoors");

            GoToStartOfTurn(env);

            AssertHitPoints(doors, 7);
        }
    }
}
