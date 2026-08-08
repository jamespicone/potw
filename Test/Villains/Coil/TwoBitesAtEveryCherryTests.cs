using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Coil
{
    [TestFixture()]
    public class TwoBitesAtEveryCherryTests : CoilTestBase
    {
        [Test()]
        public void TestVillainDamageIncreasedAndIrreducible()
        {
            SetupCoilGame();
            RemoveCoilTriggers();
            CleanupSetupNoise();

            PlayCard("TwoBitesAtEveryCherry");
            PlayCard("Fortitude"); // Reduce damage dealt to Legacy by 1

            QuickHPStorage(legacy);
            AssertIrreducible();

            DealDamage(scheming, legacy.CharacterCard, 2, DamageType.Energy);

            // 2 + 1 = 3, and Fortitude can't reduce it.
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestHeroDamageUnaffected()
        {
            SetupCoilGame();
            RemoveCoilTriggers();
            CleanupSetupNoise();

            PlayCard("TwoBitesAtEveryCherry");

            QuickHPStorage(bunker);
            AssertNotIrreducible();

            DealDamage(haka, bunker, 2, DamageType.Melee);

            QuickHPCheck(-2);
        }
    }
}
