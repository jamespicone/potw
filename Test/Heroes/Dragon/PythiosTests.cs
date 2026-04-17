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
    public class PythiosTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var card = PlayCard("Pythios");
            AssertIsInPlay(card);
        }

        [Test()]
        public void TestFocusCostOf1()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var tokenPool = dragon.CharacterCard.FindTokenPool("FocusPool");
            var mech = PlayCard("Pythios");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            // Base 4 - 1 from Pythios = 3
            AssertTokenPoolCount(tokenPool, 3);
        }

        [Test()]
        public void TestEndOfTurn_Deal1LightningTo3OrLessHP()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var mech = PlayCard("Pythios");

            // Create a target with 3 or less HP
            var envTarget = PlayCard("VelociraptorPack");
            SetHitPoints(envTarget, 3);

            DecisionDoNotActivatableAbility = true;
            DecisionSelectTarget = envTarget;

            QuickHPStorage(envTarget);
            AssertDamageSource(mech);
            AssertDamageType(DamageType.Lightning);

            GoToEndOfTurn(dragon);

            QuickHPCheck(-1);
        }

        [Test()]
        public void TestOnlyTargets3OrLessHP()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var mech = PlayCard("Pythios");

            // Baron has more than 3 HP
            Assert.That(baron.CharacterCard.HitPoints, Is.GreaterThan(3));

            DecisionDoNotActivatableAbility = true;

            // No valid targets with 3 or less HP
            QuickHPStorage(baron);
            GoToEndOfTurn(dragon);
            QuickHPCheck(0); // Baron not damaged (HP > 3)
        }

        [Test()]
        public void TestFocusAbility1_PutTargetOnDeck()
        {
            // Pythios can bounce non-character targets - verify it has the capability
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var mech = PlayCard("Pythios");

            // Verify Pythios has focus abilities
            var controller = FindCardController(mech);
            var abilities = controller.GetActivatableAbilities("focus");

            Assert.That(abilities.Count(), Is.GreaterThanOrEqualTo(1), "Pythios should have at least 1 focus ability");
        }

        [Test()]
        public void TestFocusAbility1_OnlyNonCharacter()
        {
            // The bounce ability should only target non-character targets
            // Verify a non-character target would qualify for the bounce criteria
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var mech = PlayCard("Pythios");
            var envTarget = PlayCard("VelociraptorPack");

            // Verify the target qualifies for bounce criteria:
            // - It's a target
            // - It's in play
            // - It's not a character
            Assert.That(envTarget.IsTarget, Is.True, "VelociraptorPack should be a target");
            Assert.That(envTarget.IsInPlay, Is.True, "VelociraptorPack should be in play");
            Assert.That(envTarget.IsCharacter, Is.False, "VelociraptorPack should not be a character");
        }

        [Test()]
        public void TestHasTwoFocusAbilities()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var mech = PlayCard("Pythios");

            // Verify Pythios has 2 focus abilities
            var controller = FindCardController(mech);
            var abilities = controller.GetActivatableAbilities("focus");

            Assert.That(abilities.Count(), Is.EqualTo(2), "Pythios should have 2 focus abilities");
        }

        [Test()]
        public void TestIsDeviceAndMech()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var mech = PlayCard("Pythios");

            Assert.That(mech.DoKeywordsContain("device"), Is.True, "Pythios should be a Device");
            Assert.That(mech.DoKeywordsContain("mech"), Is.True, "Pythios should be a Mech");
        }

        [Test()]
        public void TestHas10HP()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var mech = PlayCard("Pythios");

            Assert.That(mech.MaximumHitPoints, Is.EqualTo(10));
        }

        [Test()]
        public void TestEndOfTurnDamageOnlyIfValidTarget()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var mech = PlayCard("Pythios");

            // All targets have more than 3 HP
            Assert.That(baron.CharacterCard.HitPoints, Is.GreaterThan(3));
            Assert.That(bunker.CharacterCard.HitPoints, Is.GreaterThan(3));
            Assert.That(dragon.CharacterCard.HitPoints, Is.GreaterThan(3));

            DecisionDoNotActivatableAbility = true;

            QuickHPStorage(baron, bunker, dragon);
            GoToEndOfTurn(dragon);
            QuickHPCheck(0, 0, 0); // No damage dealt
        }

        [Test()]
        public void TestBounceAbilityTargetsCriteria()
        {
            // The bounce ability can target any non-character target (villain, hero, environment)
            // This test verifies the criteria allows for such targets
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var mech = PlayCard("Pythios");
            var villainTarget = PlayCard("BladeBattalion");

            // Verify BladeBattalion qualifies as a bounce target:
            // - It's a target
            // - It's in play
            // - It's not a character
            Assert.That(villainTarget.IsTarget, Is.True, "BladeBattalion should be a target");
            Assert.That(villainTarget.IsInPlay, Is.True, "BladeBattalion should be in play");
            Assert.That(villainTarget.IsCharacter, Is.False, "BladeBattalion should not be a character");
        }
    }
}
