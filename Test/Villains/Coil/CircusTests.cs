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
    public class CircusTests : CoilTestBase
    {
        [Test()]
        public void TestHeroPlayingCardTakesDamage()
        {
            SetupCoilGame();
            RemoveCoilTriggers();
            CleanupSetupNoise();

            var circus = PlayCard("Circus");

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageType(DamageType.Projectile);
            AssertDamageSource(circus);

            PlayCard("DangerSense");

            QuickHPCheck(-1, 0, 0);
        }

        [Test()]
        public void TestVillainCardPlayDoesNotTriggerDamage()
        {
            SetupCoilGame();
            RemoveCoilTriggers();
            CleanupSetupNoise();

            PlayCard("Circus");

            QuickHPStorage(legacy, bunker, haka);

            PlayCard("Mercenaries");

            QuickHPCheck(0, 0, 0);
        }
    }
}
