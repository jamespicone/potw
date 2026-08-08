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
    public class TrainwreckTests : CoilTestBase
    {
        [Test()]
        public void TestRedirectsDamageFromOtherVillainTargets()
        {
            SetupCoilGame();
            RemoveCoilTriggers();
            CleanupSetupNoise();

            var trainwreck = PlayCard("Trainwreck");

            QuickHPStorage(trainwreck, scheming);

            DealDamage(haka, scheming, 3, DamageType.Melee);

            QuickHPCheck(-3, 0);
        }

        [Test()]
        public void TestRegainsHPAtStartOfVillainTurn()
        {
            SetupCoilGame();
            RemoveCoilTriggers();
            CleanupSetupNoise();

            var trainwreck = PlayCard("Trainwreck");
            SetHitPoints(trainwreck, 5);

            GoToEndOfTurn();
            GoToStartOfTurn(coil);

            // Regains H = 3 HP.
            AssertHitPoints(trainwreck, 8);
        }
    }
}
