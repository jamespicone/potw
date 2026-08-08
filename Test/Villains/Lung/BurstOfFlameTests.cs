using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Lung
{
    [TestFixture()]
    public class BurstOfFlameTests : LungTestBase
    {
        [Test()]
        public void TestDamagesHighestHeroWithEmptyTrash()
        {
            SetupLungGame();
            RemoveLungTriggers();

            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 15);
            SetHitPoints(haka, 25);

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageSource(lung.CharacterCard);
            AssertDamageType(DamageType.Fire);

            PlayCard("BurstOfFlame");

            // X = 1 + 0 / 2 = 1
            QuickHPCheck(0, 0, -1);
        }

        [Test()]
        public void TestDamageScalesWithTrash()
        {
            SetupLungGame();
            RemoveLungTriggers();
            FillLungTrash(6);

            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 15);
            SetHitPoints(haka, 25);

            QuickHPStorage(legacy, bunker, haka);

            PlayCard("BurstOfFlame");

            // X = 1 + 6 / 2 = 4
            QuickHPCheck(0, 0, -4);
        }

        [Test()]
        public void TestDamageRoundsDown()
        {
            SetupLungGame();
            RemoveLungTriggers();
            FillLungTrash(5);

            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 15);
            SetHitPoints(haka, 25);

            QuickHPStorage(legacy, bunker, haka);

            PlayCard("BurstOfFlame");

            // X = 1 + 5 / 2 = 3 (rounds down)
            QuickHPCheck(0, 0, -3);
        }
    }
}
