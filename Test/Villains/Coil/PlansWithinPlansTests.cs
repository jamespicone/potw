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
    public class PlansWithinPlansTests : CoilTestBase
    {
        [Test()]
        public void TestReducesDamageToVillainTargets()
        {
            SetupCoilGame();
            RemoveCoilTriggers();
            CleanupSetupNoise();

            PlayCard("PlansWithinPlans");

            QuickHPStorage(scheming);
            DealDamage(haka, scheming, 5, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestDoesNotReduceDamageToHeroes()
        {
            SetupCoilGame();
            RemoveCoilTriggers();
            CleanupSetupNoise();

            PlayCard("PlansWithinPlans");

            QuickHPStorage(haka);
            DealDamage(scheming, haka.CharacterCard, 5, DamageType.Energy);
            QuickHPCheck(-5);
        }
    }
}
