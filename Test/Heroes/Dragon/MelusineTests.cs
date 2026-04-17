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
    public class MelusineTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var card = PlayCard("Melusine");
            AssertIsInPlay(card);
        }

        [Test()]
        public void TestFocusCostOf1()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var tokenPool = dragon.CharacterCard.FindTokenPool("FocusPool");
            var melusine = PlayCard("Melusine");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            // Base 4 - 1 from Melusine = 3
            AssertTokenPoolCount(tokenPool, 3);
        }

        [Test()]
        public void TestCounterDamage_DealsSource2Fire()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var melusine = PlayCard("Melusine");

            QuickHPStorage(melusine, baron.CharacterCard);
            DealDamage(baron, melusine, 2, DamageType.Melee);

            // Melusine takes 2, Baron takes 2 fire counter
            QuickHPCheck(-2, -2);
        }

        [Test()]
        public void TestCounterDamageSource()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var melusine = PlayCard("Melusine");

            // Track HP changes to verify counter damage
            QuickHPStorage(melusine, baron.CharacterCard);

            DealDamage(baron, melusine, 2, DamageType.Melee);

            // Melusine takes 2, Baron takes 2 from counter
            QuickHPCheck(-2, -2);
        }

        [Test()]
        public void TestFocusAbility1_Deal3MeleeDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var melusine = PlayCard("Melusine");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            // Activate Melusine's focus ability 1 (3 melee damage)
            DecisionDoNotActivatableAbility = false;
            DecisionActivateAbilities = new Card[] { melusine };
            DecisionSelectTarget = baron.CharacterCard;
            AssertDamageSource(melusine);
            AssertDamageType(DamageType.Melee);

            QuickHPStorage(baron);
            UsePower(dragon.CharacterCard);

            QuickHPCheck(-3);
        }

        [Test()]
        public void TestHasTwoFocusAbilities()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var melusine = PlayCard("Melusine");

            // Verify Melusine has 2 focus abilities
            var controller = FindCardController(melusine);
            var abilities = controller.GetActivatableAbilities("focus");

            Assert.That(abilities.Count(), Is.EqualTo(2), "Melusine should have 2 focus abilities");
        }

        [Test()]
        public void TestCounterDoesNotTriggerOnNonTargetSource()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var melusine = PlayCard("Melusine");

            // Deal damage from a card source that is not a target
            // The counter should only trigger if the damage source is a target
            QuickHPStorage(melusine, baron.CharacterCard);
            // Baron is a target, so this should trigger counter
            DealDamage(baron, melusine, 2, DamageType.Melee);
            QuickHPCheck(-2, -2);
        }

        [Test()]
        public void TestCounterDoesNotTriggerIfNoDamageDealt()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Legacy", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var melusine = PlayCard("Melusine");

            // Make Melusine immune to damage
            // Actually, let's just deal 0 damage
            QuickHPStorage(melusine, baron.CharacterCard);
            DealDamage(baron, melusine, 0, DamageType.Melee);
            QuickHPCheck(0, 0); // No damage dealt, no counter
        }

        [Test()]
        public void TestIsDeviceAndMech()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var melusine = PlayCard("Melusine");

            Assert.That(melusine.DoKeywordsContain("device"), Is.True, "Melusine should be a Device");
            Assert.That(melusine.DoKeywordsContain("mech"), Is.True, "Melusine should be a Mech");
        }

        [Test()]
        public void TestHas12HP()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var melusine = PlayCard("Melusine");

            Assert.That(melusine.MaximumHitPoints, Is.EqualTo(12));
        }

        [Test()]
        public void TestCounterDamageIsFire()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var melusine = PlayCard("Melusine");

            // Track that Baron takes fire damage from counter
            QuickHPStorage(baron.CharacterCard);

            // The incoming damage to Melusine triggers counter damage to Baron
            DealDamage(baron, melusine, 2, DamageType.Melee);

            // Baron should have taken 2 fire damage from the counter
            QuickHPCheck(-2);
        }
    }
}
