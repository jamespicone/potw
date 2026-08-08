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
    public class DinahTests : CoilTestBase
    {
        [Test()]
        public void TestSchemingAndActingImmuneToDamage()
        {
            SetupCoilGame();
            RemoveCoilTriggers();
            CleanupSetupNoise();

            var dinah = PlayCard("Dinah");

            QuickHPStorage(scheming, acting, dinah);

            DealDamage(haka, scheming, 3, DamageType.Melee);
            DealDamage(haka, acting, 3, DamageType.Melee);

            // Dinah herself is not immune.
            DealDamage(haka, dinah, 1, DamageType.Melee);

            QuickHPCheck(0, 0, -1);
        }

        [Test()]
        public void TestDamageDuringHeroTurnForcesDiscard()
        {
            SetupCoilGame();
            RemoveCoilTriggers();
            CleanupSetupNoise();

            var dinah = PlayCard("Dinah");

            GoToPlayCardPhase(legacy);

            QuickHandStorage(legacy);
            DealDamage(legacy, dinah, 1, DamageType.Melee);
            QuickHandCheck(-1);
        }

        [Test()]
        public void TestDamageDuringVillainTurnNoDiscard()
        {
            SetupCoilGame();
            RemoveCoilTriggers();
            CleanupSetupNoise();

            var dinah = PlayCard("Dinah");

            QuickHandStorage(legacy);
            DealDamage(legacy, dinah, 1, DamageType.Melee);
            QuickHandCheck(0);
        }
    }
}
