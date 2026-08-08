using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Echidna
{
    [TestFixture()]
    public class RoutTests : ParahumanTest
    {
        private void SetupRoutGame()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "InsulaPrimalis"
            );
            StartGame();
            RemoveVillainTriggers();
            DestroyNonCharacterVillainCards();
            ReturnAllTwisted();
        }

        [Test()]
        public void TestGainsChargeTokenAtEndOfTurn()
        {
            SetupRoutGame();

            var rout = PlayCard("RoutTwisted");

            GoToEndOfTurn();

            Assert.That(rout.FindTokenPool("ChargePool").CurrentValue, Is.EqualTo(1));
        }

        [Test()]
        public void TestStartOfTurnDamageScalesWithCharges()
        {
            SetupRoutGame();

            var rout = PlayCard("RoutTwisted");
            rout.FindTokenPool("ChargePool").SetNumberOfTokens(2);

            SetHitPoints(alexandria, 25);
            SetHitPoints(bitch, 15);

            QuickHPStorage(alexandria, bitch);
            AssertDamageType(DamageType.Energy);

            // The end of the villain turn adds one more charge (2 -> 3) before
            // the start-of-turn attack fires.
            GoToEndOfTurn();
            GoToStartOfTurn(echidna);

            // X = 2 + 3 charges.
            QuickHPCheck(-5, 0);
        }
    }
}
