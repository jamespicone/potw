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
    public class SundancerTests : CoilTestBase
    {
        [Test()]
        public void TestImmuneToFireDamage()
        {
            SetupCoilGame();
            RemoveCoilTriggers();
            CleanupSetupNoise();

            var sundancer = PlayCard("Sundancer");

            QuickHPStorage(sundancer);

            DealDamage(haka, sundancer, 3, DamageType.Fire);
            QuickHPCheck(0);

            DealDamage(haka, sundancer, 3, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestEndOfTurnDamagesAllHeroes()
        {
            SetupCoilGame();
            RemoveCoilTriggers();
            CleanupSetupNoise();

            var sundancer = PlayCard("Sundancer");

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageType(DamageType.Fire, DamageType.Fire, DamageType.Fire);
            AssertDamageSource(sundancer, sundancer, sundancer);

            GoToEndOfTurn();

            QuickHPCheck(-2, -2, -2);
        }
    }
}
