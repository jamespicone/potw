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
    public class GlaurungTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var card = PlayCard("Glaurung");
            AssertIsInPlay(card);
        }

        [Test()]
        public void TestFocusCostOf1()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var tokenPool = dragon.CharacterCard.FindTokenPool("FocusPool");
            var mech = PlayCard("Glaurung");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            // Base 4 - 1 from Glaurung = 3
            AssertTokenPoolCount(tokenPool, 3);
        }

        [Test()]
        public void TestEndOfTurn_PlacesDroneToken()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var mech = PlayCard("Glaurung");
            var dronePool = mech.FindTokenPool("DronePool");

            AssertTokenPoolCount(dronePool, 0);

            DecisionDoNotActivatableAbility = true;
            GoToEndOfTurn(dragon);

            AssertTokenPoolCount(dronePool, 1);
        }

        [Test()]
        public void TestTakesDamage_Removes2DroneTokens()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var mech = PlayCard("Glaurung");
            var dronePool = mech.FindTokenPool("DronePool");

            // Add some Drone tokens
            AddTokensToPool(dronePool, 5);
            AssertTokenPoolCount(dronePool, 5);

            // Deal damage to Glaurung
            DealDamage(baron, mech, 2, DamageType.Melee);

            // Should lose 2 drone tokens
            AssertTokenPoolCount(dronePool, 3);
        }

        [Test()]
        public void TestTakesDamageMultipleTimes_LosesTokensEachTime()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var mech = PlayCard("Glaurung");
            var dronePool = mech.FindTokenPool("DronePool");

            // Add some Drone tokens
            AddTokensToPool(dronePool, 6);
            AssertTokenPoolCount(dronePool, 6);

            // Deal damage twice
            DealDamage(baron, mech, 1, DamageType.Melee);
            AssertTokenPoolCount(dronePool, 4);

            DealDamage(baron, mech, 1, DamageType.Melee);
            AssertTokenPoolCount(dronePool, 2);
        }

        [Test()]
        public void TestFocusAbility1_DealXTargets2Lightning()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var mech = PlayCard("Glaurung");
            var dronePool = mech.FindTokenPool("DronePool");

            // Add 2 Drone tokens - can hit up to 2 targets
            AddTokensToPool(dronePool, 2);

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            // Activate focus ability (deal 2 lightning to X targets)
            DecisionDoNotActivatableAbility = false;
            DecisionActivateAbilities = new Card[] { mech };
            DecisionSelectTargets = new Card[] { baron.CharacterCard, bunker.CharacterCard };

            QuickHPStorage(baron, bunker);
            AssertDamageSource(mech);
            AssertDamageType(DamageType.Lightning, DamageType.Lightning);

            UsePower(dragon.CharacterCard);

            QuickHPCheck(-2, -2);
        }

        [Test()]
        public void TestDamageScalesWithTokens()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "Legacy", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var mech = PlayCard("Glaurung");
            var dronePool = mech.FindTokenPool("DronePool");

            // Add 3 Drone tokens - can hit up to 3 targets
            AddTokensToPool(dronePool, 3);

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            DecisionDoNotActivatableAbility = false;
            DecisionActivateAbilities = new Card[] { mech };
            DecisionSelectTargets = new Card[] { baron.CharacterCard, bunker.CharacterCard, legacy.CharacterCard };

            QuickHPStorage(baron, bunker, legacy);
            UsePower(dragon.CharacterCard);

            QuickHPCheck(-2, -2, -2); // All three take 2 damage
        }

        [Test()]
        public void TestNoTokens_NoTargets()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var mech = PlayCard("Glaurung");
            var dronePool = mech.FindTokenPool("DronePool");

            AssertTokenPoolCount(dronePool, 0);

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            DecisionDoNotActivatableAbility = false;
            DecisionActivateAbilities = new Card[] { mech };

            QuickHPStorage(baron);
            UsePower(dragon.CharacterCard);

            // With 0 tokens, can't select any targets
            QuickHPCheck(0);
        }

        [Test()]
        public void TestIsDeviceAndMech()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var mech = PlayCard("Glaurung");

            Assert.That(mech.DoKeywordsContain("device"), Is.True, "Glaurung should be a Device");
            Assert.That(mech.DoKeywordsContain("mech"), Is.True, "Glaurung should be a Mech");
        }

        [Test()]
        public void TestHas10HP()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var mech = PlayCard("Glaurung");

            Assert.That(mech.MaximumHitPoints, Is.EqualTo(10));
        }

        [Test()]
        public void TestCannotLoseMoreTokensThanHave()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var mech = PlayCard("Glaurung");
            var dronePool = mech.FindTokenPool("DronePool");

            // Add only 1 Drone token
            AddTokensToPool(dronePool, 1);
            AssertTokenPoolCount(dronePool, 1);

            // Deal damage - would remove 2 but only 1 exists
            DealDamage(baron, mech, 1, DamageType.Melee);

            // Should be at minimum (0)
            AssertTokenPoolCount(dronePool, 0);
        }
    }
}
