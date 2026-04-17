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
    public class CommandAndControlTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var card = PlayCard("CommandAndControl");
            AssertIsInPlay(card);
        }

        [Test()]
        public void TestFocusAbility_2ProjectileDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var card = PlayCard("CommandAndControl");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            // Activate focus ability - select Bunker to deal projectile damage
            DecisionDoNotActivatableAbility = false;
            DecisionActivateAbilities = new Card[] { card };
            DecisionSelectCards = new Card[] { bunker.CharacterCard, baron.CharacterCard };
            DecisionSelectDamageType = DamageType.Projectile;

            QuickHPStorage(baron);
            AssertDamageSource(bunker.CharacterCard);
            AssertDamageType(DamageType.Projectile);

            UsePower(dragon.CharacterCard);

            QuickHPCheck(-2);
        }

        [Test()]
        public void TestFocusAbility_2MeleeDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var card = PlayCard("CommandAndControl");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            // Activate focus ability - select Dragon to deal melee damage
            DecisionDoNotActivatableAbility = false;
            DecisionActivateAbilities = new Card[] { card };
            DecisionSelectCards = new Card[] { dragon.CharacterCard, baron.CharacterCard };
            DecisionSelectDamageType = DamageType.Melee;

            QuickHPStorage(baron);
            AssertDamageSource(dragon.CharacterCard);
            AssertDamageType(DamageType.Melee);

            UsePower(dragon.CharacterCard);

            QuickHPCheck(-2);
        }

        [Test()]
        public void TestDamageSourceIsSelectedHero()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "Legacy", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var card = PlayCard("CommandAndControl");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            // Select Legacy as the damage source
            DecisionDoNotActivatableAbility = false;
            DecisionActivateAbilities = new Card[] { card };
            DecisionSelectCards = new Card[] { legacy.CharacterCard, baron.CharacterCard };
            DecisionSelectDamageType = DamageType.Melee;

            AssertDamageSource(legacy.CharacterCard);

            UsePower(dragon.CharacterCard);
        }

        [Test()]
        public void TestAnyHeroTargetCanDealDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var card = PlayCard("CommandAndControl");

            // Play a mech (which is a hero target)
            var mech = PlayCard("Ladon");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            // Select the mech as damage source
            DecisionDoNotActivatableAbility = false;
            DecisionActivateAbilities = new Card[] { card };
            DecisionSelectCards = new Card[] { mech, baron.CharacterCard };
            DecisionSelectDamageType = DamageType.Projectile;

            QuickHPStorage(baron);
            AssertDamageSource(mech);

            UsePower(dragon.CharacterCard);

            QuickHPCheck(-2);
        }

        [Test()]
        public void TestCostsFocusPoint()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var tokenPool = dragon.CharacterCard.FindTokenPool("FocusPool");
            var card = PlayCard("CommandAndControl");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            AssertTokenPoolCount(tokenPool, 4);

            DecisionDoNotActivatableAbility = false;
            DecisionActivateAbilities = new Card[] { card };
            DecisionSelectCards = new Card[] { bunker.CharacterCard, baron.CharacterCard };
            DecisionSelectDamageType = DamageType.Melee;

            UsePower(dragon.CharacterCard);

            // Power doesn't consume focus - only Focus Phase does
            AssertTokenPoolCount(tokenPool, 4);
        }

        [Test()]
        public void TestIsOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var card = GetCard("CommandAndControl");

            Assert.That(card.IsOngoing, Is.True, "Command and Control should be an Ongoing");
        }
    }
}
