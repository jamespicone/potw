using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Bitch
{
    [TestFixture()]
    public class KillTests : ParahumanTest
    {
        [Test()]
        public void TestKillNoDogs()
        {
            // Arrange
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            // Get player's character card and Baron Blade's character card
            Card bitch = GetCard("BitchCharacter");
            Card baron = GetCard("BaronBladeCharacter");

            // Put Kill card in hand
            Card kill = PutInHand("Kill");

            // Select Baron Blade as target
            DecisionSelectTarget = baron;

            // Act - play Kill
            PlayCard(kill);

            // Assert - with no dogs in play, should deal 0 damage (3 * 0)
            AssertIsAtMaxHP(baron);
        }

        [Test()]
        public void TestKillWithOneDog()
        {
            // Arrange
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            Card brutus = PlayCard("Brutus");
            Card baron = GetCard("BaronBladeCharacter");
            Card kill = PutInHand("Kill");

            DecisionSelectTarget = baron;
            QuickHPStorage(baron);

            AssertDamageSource(bitch.CharacterCard);
            AssertDamageType(DamageType.Melee);

            // Act
            PlayCard(kill);

            // Assert - with 1 dog in play, should deal 3 damage (3 * 1)
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestKillWithThreeDogs()
        {
            // Arrange
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            Card brutus = PlayCard("Brutus");
            Card judas = PlayCard("Judas");
            Card angelica = PlayCard("Angelica");
            Card baron = GetCard("BaronBladeCharacter");
            Card kill = PutInHand("Kill");

            DecisionSelectTarget = baron;
            QuickHPStorage(baron);

            AssertDamageSource(bitch.CharacterCard);
            AssertDamageType(DamageType.Melee);

            // Act
            PlayCard(kill);

            // Assert - with 3 dogs in play, should deal 9 damage (3 * 3) 
            QuickHPCheck(-9);
        }

        [Test()]
        public void TestKillWithDestroyedDogs()
        {
            // Arrange
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            Card brutus = PlayCard("Brutus");
            Card judas = PlayCard("Judas");
            Card angelica = PlayCard("Angelica");
            Card baron = GetCard("BaronBladeCharacter");
            Card kill = PutInHand("Kill");

            // Destroy one dog
            DestroyCard(brutus);

            DecisionSelectTarget = baron;
            QuickHPStorage(baron);

            AssertDamageSource(bitch.CharacterCard);
            AssertDamageType(DamageType.Melee);

            // Act
            PlayCard(kill);

            // Assert - with 2 dogs in play (1 destroyed), should deal 6 damage (3 * 2)
            QuickHPCheck(-6);
        }
    }
}