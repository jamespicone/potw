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
    public class MercenariesTests : CoilTestBase
    {
        [Test()]
        public void TestEndOfTurnDamagesTwoHighestHeroes()
        {
            SetupCoilGame();
            RemoveCoilTriggers();
            CleanupSetupNoise();

            var mercs = PlayCard("Mercenaries");

            SetHitPoints(legacy, 30);
            SetHitPoints(bunker, 10);
            SetHitPoints(haka, 28);

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageType(DamageType.Energy, DamageType.Energy);
            AssertDamageSource(mercs, mercs);

            GoToEndOfTurn();

            // H - 1 = 2 energy damage to the two highest hero targets.
            QuickHPCheck(-2, 0, -2);
        }
    }
}
