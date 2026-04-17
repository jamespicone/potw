using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Dragon
{
    [TestFixture()]
    public class AstarothNidhugTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var card = PlayCard("AstarothNidhug");
            AssertIsInPlay(card);
        }

        [Test()]
        public void TestFocusCostOf1()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var tokenPool = dragon.CharacterCard.FindTokenPool("FocusPool");
            var mech = PlayCard("AstarothNidhug");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            // Base 4 - 1 from Astaroth-Nidhug = 3
            AssertTokenPoolCount(tokenPool, 3);
        }

        [Test()]
        public void TestEndOfTurn_PlacesAimToken()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var mech = PlayCard("AstarothNidhug");
            var aimPool = mech.FindTokenPool("AimPool");

            AssertTokenPoolCount(aimPool, 0);

            DecisionDoNotActivatableAbility = true;
            GoToEndOfTurn(dragon);

            AssertTokenPoolCount(aimPool, 1);
        }

        [Test()]
        public void TestHasTwoFocusAbilities()
        {
            // Astaroth-Nidhug has 2 focus abilities:
            // 1. Place an Aim token
            // 2. Deal X projectile damage (X = Aim tokens)
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var mech = PlayCard("AstarothNidhug");
            var controller = FindCardController(mech);
            var abilities = controller.GetActivatableAbilities("focus");

            Assert.That(abilities.Count(), Is.EqualTo(2), "Astaroth-Nidhug should have 2 focus abilities");
        }

        [Test()]
        public void TestAimTokenPoolExists()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var mech = PlayCard("AstarothNidhug");
            var aimPool = mech.FindTokenPool("AimPool");

            Assert.That(aimPool, Is.Not.Null, "Astaroth-Nidhug should have an AimPool");
            AssertTokenPoolCount(aimPool, 0); // Starts at 0
        }

        [Test()]
        public void TestEndOfTurnAccumulatesTokens()
        {
            // Test that tokens accumulate over multiple turns
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var mech = PlayCard("AstarothNidhug");
            var aimPool = mech.FindTokenPool("AimPool");

            AssertTokenPoolCount(aimPool, 0);

            // First end of turn
            DecisionDoNotActivatableAbility = true;
            GoToEndOfTurn(dragon);
            AssertTokenPoolCount(aimPool, 1);

            // Second end of turn
            GoToEndOfTurn(dragon);
            AssertTokenPoolCount(aimPool, 2);

            // Third end of turn
            GoToEndOfTurn(dragon);
            AssertTokenPoolCount(aimPool, 3);
        }

        [Test()]
        public void TestIsDeviceAndMech()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var mech = PlayCard("AstarothNidhug");

            Assert.That(mech.DoKeywordsContain("device"), Is.True, "Astaroth-Nidhug should be a Device");
            Assert.That(mech.DoKeywordsContain("mech"), Is.True, "Astaroth-Nidhug should be a Mech");
        }

        [Test()]
        public void TestHas10HP()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var mech = PlayCard("AstarothNidhug");

            Assert.That(mech.MaximumHitPoints, Is.EqualTo(10));
        }
    }
}
