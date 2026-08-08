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
    public class UndergroundBaseTests : CoilTestBase
    {
        [Test()]
        public void TestVillainsImmuneToEnvironmentDamage()
        {
            SetupCoilGame();
            RemoveCoilTriggers();
            CleanupSetupNoise();

            var pileup = PlayCard("TrafficPileup");

            QuickHPStorage(scheming);
            DealDamage(pileup, scheming, 4, DamageType.Melee);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestVillainsNotImmuneToHeroDamage()
        {
            SetupCoilGame();
            RemoveCoilTriggers();
            CleanupSetupNoise();

            QuickHPStorage(scheming);
            DealDamage(haka, scheming, 4, DamageType.Melee);
            QuickHPCheck(-4);
        }

        [Test()]
        public void TestHeroesNotProtectedFromEnvironmentDamage()
        {
            SetupCoilGame();
            RemoveCoilTriggers();
            CleanupSetupNoise();

            var pileup = PlayCard("TrafficPileup");

            QuickHPStorage(haka);
            DealDamage(pileup, haka.CharacterCard, 4, DamageType.Melee);
            QuickHPCheck(-4);
        }
    }
}
