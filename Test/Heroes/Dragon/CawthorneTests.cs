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
    public class CawthorneTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var card = PlayCard("Cawthorne");
            AssertIsInPlay(card);
        }

        [Test()]
        public void TestFocusCostOf1()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var tokenPool = dragon.CharacterCard.FindTokenPool("FocusPool");
            var cawthorne = PlayCard("Cawthorne");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            // Base 4 - 1 from Cawthorne = 3
            AssertTokenPoolCount(tokenPool, 3);
        }

        [Test()]
        public void TestEndOfTurn_Deal1ProjectileTo2Targets()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var cawthorne = PlayCard("Cawthorne");

            DecisionDoNotActivatableAbility = true;

            // Select 2 targets for end of turn damage
            DecisionSelectTargets = new Card[] { baron.CharacterCard, bunker.CharacterCard };
            QuickHPStorage(baron, bunker);

            GoToEndOfTurn(dragon);

            // Both targets should take 1 damage each
            QuickHPCheck(-1, -1);
        }

        [Test()]
        public void TestEndOfTurnDamageIsOptional()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var cawthorne = PlayCard("Cawthorne");

            // Reset all decisions to ensure no leakage from previous tests
            ResetDecisions();
            DecisionDoNotActivatableAbility = true;

            // Select only 1 target (can select 0-2), then skip second selection
            DecisionSelectTargets = new Card[] { baron.CharacterCard, null };
            QuickHPStorage(baron, bunker);

            GoToEndOfTurn(dragon);

            QuickHPCheck(-1, 0); // Only baron takes damage
        }

        [Test()]
        public void TestFocusAbility_ReduceNextDamageBy2()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var cawthorne = PlayCard("Cawthorne");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            // Activate Cawthorne's focus ability - select Baron, then skip second selection
            DecisionDoNotActivatableAbility = false;
            DecisionActivateAbilities = new Card[] { cawthorne };
            DecisionSelectCards = new Card[] { baron.CharacterCard, null }; // Select Baron, skip second
            UsePower(dragon.CharacterCard);

            // Baron's next damage should be reduced by 2
            QuickHPStorage(dragon);
            DealDamage(baron, dragon, 4, DamageType.Melee);
            QuickHPCheck(-2); // 4 - 2 = 2
        }

        [Test()]
        public void TestReductionAppliesOnce()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var cawthorne = PlayCard("Cawthorne");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            // Activate Cawthorne's focus ability - select Baron, then skip second selection
            DecisionDoNotActivatableAbility = false;
            DecisionActivateAbilities = new Card[] { cawthorne };
            DecisionSelectCards = new Card[] { baron.CharacterCard, null }; // Select Baron, skip second
            UsePower(dragon.CharacterCard);

            // First damage is reduced
            QuickHPStorage(dragon);
            DealDamage(baron, dragon, 4, DamageType.Melee);
            QuickHPCheck(-2); // 4 - 2 = 2

            // Second damage is NOT reduced
            QuickHPStorage(dragon);
            DealDamage(baron, dragon, 4, DamageType.Melee);
            QuickHPCheck(-4); // Full damage
        }

        [Test()]
        public void TestFocusAbilityCanSelectMultipleTargets()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var envTarget = PlayCard("VelociraptorPack");
            var cawthorne = PlayCard("Cawthorne");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            // Activate Cawthorne's focus ability - select Baron and Environment
            DecisionDoNotActivatableAbility = false;
            DecisionActivateAbilities = new Card[] { cawthorne };
            DecisionSelectCards = new Card[] { baron.CharacterCard, envTarget };
            UsePower(dragon.CharacterCard);

            // Both targets' next damage should be reduced
            QuickHPStorage(dragon);
            DealDamage(baron, dragon, 4, DamageType.Melee);
            QuickHPCheck(-2);

            QuickHPStorage(dragon);
            DealDamage(envTarget, dragon, 4, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestIsDeviceAndMech()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var cawthorne = PlayCard("Cawthorne");

            Assert.That(cawthorne.DoKeywordsContain("device"), Is.True, "Cawthorne should be a Device");
            Assert.That(cawthorne.DoKeywordsContain("mech"), Is.True, "Cawthorne should be a Mech");
        }

        [Test()]
        public void TestHas10HP()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var cawthorne = PlayCard("Cawthorne");

            Assert.That(cawthorne.MaximumHitPoints, Is.EqualTo(10));
        }
    }
}
