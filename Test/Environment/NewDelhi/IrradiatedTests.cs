using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Environment.NewDelhi
{
    [TestFixture()]
    public class IrradiatedTests : NewDelhiTestBase
    {
        [Test()]
        public void TestGainsTokenAtEndOfTurn()
        {
            SetupNewDelhiGame();

            // Play it during the environment turn (as the environment would), after
            // the start-of-turn destroy check has already passed.
            GoToPlayCardPhase(env);
            var irradiated = PlayCard("Irradiated");

            GoToEndOfTurn(env);

            Assert.That(irradiated.FindTokenPool("IrradiatedPool").CurrentValue, Is.EqualTo(1));
        }

        [Test()]
        public void TestStartOfTurnDamageScalesWithTokens()
        {
            SetupNewDelhiGame();

            var irradiated = PlayCard("Irradiated");
            irradiated.FindTokenPool("IrradiatedPool").SetNumberOfTokens(2);

            QuickHPStorage(baron.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, haka.CharacterCard);
            AssertDamageType(DamageType.Energy);

            GoToStartOfTurn(env);

            QuickHPCheck(-2, -2, -2, -2);
        }

        [Test()]
        public void TestDestroyedAtStartOfTurnWithNoTokens()
        {
            SetupNewDelhiGame();

            var irradiated = PlayCard("Irradiated");

            GoToStartOfTurn(env);

            AssertInTrash(irradiated);
        }
    }
}
