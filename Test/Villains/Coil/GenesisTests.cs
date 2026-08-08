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
    public class GenesisTests : CoilTestBase
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.Coil", "Tempest", "InsulaPrimalis");
        }

        [Test()]
        public void DestroyedByEnvDamage()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Coil",
                "Legacy",
                "InsulaPrimalis"
            );

            StartGame();
            RemoveVillainCards();
            var genesis = PlayCard("Genesis");

            DealDamage(FindEnvironment().TurnTaker, genesis, 20, DamageType.Infernal);

            AssertInDeck(genesis);
        }

        [Test()]
        public void TestEndOfTurnDamagesHHeroes()
        {
            SetupCoilGame();
            RemoveCoilTriggers();
            CleanupSetupNoise();

            var genesis = PlayCard("Genesis");

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageType(DamageType.Toxic, DamageType.Toxic, DamageType.Toxic);
            AssertDamageSource(genesis, genesis, genesis);

            GoToEndOfTurn();

            // 2 toxic damage to H = 3 hero targets.
            QuickHPCheck(-2, -2, -2);
        }
    }
}
