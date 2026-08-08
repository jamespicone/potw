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
    public class BallisticTests : CoilTestBase
    {
        [Test()]
        public void TestDamagingHeroCharacterDestroysTheirCard()
        {
            SetupCoilGame();
            RemoveCoilTriggers();
            CleanupSetupNoise();

            var ballistic = PlayCard("Ballistic");
            var sense = PlayCard("DangerSense");

            DealDamage(ballistic, legacy.CharacterCard, 2, DamageType.Projectile);

            AssertInTrash(sense);
        }

        [Test()]
        public void TestEndOfTurnDamagesHighestHero()
        {
            SetupCoilGame();
            RemoveCoilTriggers();
            CleanupSetupNoise();

            var ballistic = PlayCard("Ballistic");

            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 15);
            SetHitPoints(haka, 25);

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageType(DamageType.Projectile);
            AssertDamageSource(ballistic);

            GoToEndOfTurn();

            // H + 1 = 4 projectile damage to the highest hero target.
            QuickHPCheck(0, 0, -4);
        }
    }
}
