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
    public class TrappedChamberTests : CoilsBaseTestBase
    {
        [Test()]
        public void TestHeroesCannotUsePowers()
        {
            SetupCoilsBaseGame();

            PlayCard("TrappedChamber");

            AssertNumberOfUsablePowers(legacy, 0);
            AssertNumberOfUsablePowers(bunker, 0);
            AssertNumberOfUsablePowers(haka, 0);
        }

        [Test()]
        public void TestTrapFiresAtStartOfEnvironmentTurn()
        {
            SetupCoilsBaseGame();

            var chamber = PlayCard("TrappedChamber");

            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 10);
            SetHitPoints(haka, 25);

            QuickHPStorage(baron.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, haka.CharacterCard);

            GoToStartOfTurn(env);

            // 2 projectile to the H = 3 highest non-environment targets, then self-destructs.
            QuickHPCheck(-2, -2, 0, -2);
            AssertInTrash(chamber);
        }
    }
}
