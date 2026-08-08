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
    public class LaserRiflesTests : CoilsBaseTestBase
    {
        [Test()]
        public void TestVillainDamageIncreasedAndBecomesEnergy()
        {
            SetupCoilsBaseGame();

            PlayCard("LaserRifles");

            QuickHPStorage(bunker);
            AssertDamageType(DamageType.Energy);

            DealDamage(baron.CharacterCard, bunker.CharacterCard, 2, DamageType.Melee);

            QuickHPCheck(-3);
        }

        [Test()]
        public void TestHeroDamageUnaffected()
        {
            SetupCoilsBaseGame();

            PlayCard("LaserRifles");

            QuickHPStorage(bunker);
            AssertDamageType(DamageType.Melee);

            DealDamage(haka, bunker, 2, DamageType.Melee);

            QuickHPCheck(-2);
        }
    }
}
