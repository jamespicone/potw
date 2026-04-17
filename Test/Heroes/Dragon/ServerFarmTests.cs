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
    public class ServerFarmTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var card = PlayCard("ServerFarm");
            AssertIsInPlay(card);
        }

        [Test()]
        public void TestStartOfTurnGains1Focus()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var tokenPool = dragon.CharacterCard.FindTokenPool("FocusPool");
            var serverFarm = PlayCard("ServerFarm");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            // Base 4 focus + 1 from Server Farm = 5
            AssertTokenPoolCount(tokenPool, 5);
        }

        [Test()]
        public void TestMultipleServerFarmsStack()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var tokenPool = dragon.CharacterCard.FindTokenPool("FocusPool");
            PlayCard("ServerFarm", 0);
            PlayCard("ServerFarm", 1);

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            // Base 4 focus + 1 from each Server Farm = 6
            AssertTokenPoolCount(tokenPool, 6);
        }

        [Test()]
        public void TestFocusAbility_DestroySelfGains3Focus()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var tokenPool = dragon.CharacterCard.FindTokenPool("FocusPool");
            var serverFarm = PlayCard("ServerFarm");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            // Base 4 + 1 from Server Farm = 5
            AssertTokenPoolCount(tokenPool, 5);

            // Use power to activate Server Farm's focus ability (destroy for 3 focus)
            DecisionDoNotActivatableAbility = false;
            DecisionActivateAbilities = new Card[] { serverFarm };
            UsePower(dragon.CharacterCard);

            // Power doesn't consume focus, so just gained 3 from destruction
            // 5 + 3 = 8
            AssertTokenPoolCount(tokenPool, 8);
            AssertInTrash(serverFarm);
        }

        [Test()]
        public void TestIsDeviceNotMech()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var serverFarm = PlayCard("ServerFarm");

            // Should have Device keyword
            Assert.That(serverFarm.DoKeywordsContain("device"), Is.True, "Server Farm should be a Device");

            // Should NOT have Mech keyword
            Assert.That(serverFarm.DoKeywordsContain("mech"), Is.False, "Server Farm should not be a Mech");
        }

        [Test()]
        public void TestDoesNotConsumeFocusLikeMechs()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var tokenPool = dragon.CharacterCard.FindTokenPool("FocusPool");
            var serverFarm = PlayCard("ServerFarm");

            // Also play a mech to compare
            var mech = PlayCard("Ladon");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            // Base 4 + 1 from Server Farm - 1 from Ladon = 4
            // Server Farm does NOT consume focus, only Mechs do
            AssertTokenPoolCount(tokenPool, 4);
        }

        [Test()]
        public void TestIsATarget()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var serverFarm = PlayCard("ServerFarm");

            Assert.That(serverFarm.IsTarget, Is.True, "Server Farm should be a target");
            Assert.That(serverFarm.MaximumHitPoints, Is.EqualTo(4), "Server Farm should have 4 HP");
        }

        [Test()]
        public void TestDestroyedByDamageDoesNotGainFocus()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var tokenPool = dragon.CharacterCard.FindTokenPool("FocusPool");
            var serverFarm = PlayCard("ServerFarm");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            // Base 4 + 1 from Server Farm = 5
            AssertTokenPoolCount(tokenPool, 5);

            // Destroy Server Farm by dealing damage (not via focus ability)
            DealDamage(baron, serverFarm, 10, DamageType.Melee);
            AssertInTrash(serverFarm);

            // Should NOT have gained 3 focus from destruction (that's only for the focus ability)
            AssertTokenPoolCount(tokenPool, 5);
        }
    }
}
