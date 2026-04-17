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
    public class LadonTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var card = PlayCard("Ladon");
            AssertIsInPlay(card);
        }

        [Test()]
        public void TestFocusCostOf1()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var tokenPool = dragon.CharacterCard.FindTokenPool("FocusPool");
            var ladon = PlayCard("Ladon");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            // Base 4 - 1 from Ladon = 3
            AssertTokenPoolCount(tokenPool, 3);
        }

        [Test()]
        public void TestReducesDamageToSelfBy1()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var ladon = PlayCard("Ladon");

            QuickHPStorage(ladon);
            DealDamage(baron, ladon, 3, DamageType.Melee);
            QuickHPCheck(-2); // 3 - 1 = 2
        }

        [Test()]
        public void TestSelfReductionDoesNotApplyToOthers()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var ladon = PlayCard("Ladon");

            // Damage to Dragon should NOT be reduced by Ladon's passive
            QuickHPStorage(dragon);
            DealDamage(baron, dragon, 3, DamageType.Melee);
            QuickHPCheck(-3); // Full damage
        }

        [Test()]
        public void TestFocusAbility_SelectDamageType()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            var tokenPool = dragon.CharacterCard.FindTokenPool("FocusPool");
            var ladon = PlayCard("Ladon");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);
            AssertTokenPoolCount(tokenPool, 3); // 4 - 1 = 3

            // Activate Ladon's focus ability to select a damage type
            DecisionDoNotActivatableAbility = false;
            DecisionActivateAbilities = new Card[] { ladon };
            DecisionSelectDamageType = DamageType.Fire;
            UsePower(dragon.CharacterCard);

            // Fire damage should now be reduced by 1
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Fire);
            QuickHPCheck(-2); // 3 - 1 = 2

            // Other damage types not reduced
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Melee);
            QuickHPCheck(-3); // Full damage
        }

        [Test()]
        public void TestFocusAbilityReducesThatTypeBy1()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            var ladon = PlayCard("Ladon");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            // Activate Ladon's focus ability - select Lightning
            DecisionDoNotActivatableAbility = false;
            DecisionActivateAbilities = new Card[] { ladon };
            DecisionSelectDamageType = DamageType.Lightning;
            UsePower(dragon.CharacterCard);

            // Lightning damage should be reduced by 1 to ALL targets
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Lightning);
            QuickHPCheck(-2);

            // Also works for Dragon
            QuickHPStorage(dragon);
            DealDamage(baron, dragon, 3, DamageType.Lightning);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestReductionLastsUntilStartOfNextTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            var ladon = PlayCard("Ladon");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            // Activate reduction for Fire
            DecisionDoNotActivatableAbility = false;
            DecisionActivateAbilities = new Card[] { ladon };
            DecisionSelectDamageType = DamageType.Fire;
            UsePower(dragon.CharacterCard);

            // Skip ability activations on next turn
            DecisionDoNotActivatableAbility = true;
            // Go to start of next Dragon's turn
            GoToStartOfTurn(dragon);

            // Reduction should be gone now
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Fire);
            QuickHPCheck(-3); // Full damage
        }

        [Test()]
        public void TestIsDeviceAndMech()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var ladon = PlayCard("Ladon");

            Assert.That(ladon.DoKeywordsContain("device"), Is.True, "Ladon should be a Device");
            Assert.That(ladon.DoKeywordsContain("mech"), Is.True, "Ladon should be a Mech");
        }

        [Test()]
        public void TestHas10HP()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var ladon = PlayCard("Ladon");

            Assert.That(ladon.MaximumHitPoints, Is.EqualTo(10));
        }
    }
}
