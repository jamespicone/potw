using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Dauntless
{
    [TestFixture()]
    public class CrystallizationTests : ParahumanTest
    {
        [Test()]
        public void TestPlayLocationOnDauntless()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var crystal = PutInHand("Crystallization");
            DecisionSelectCard = dauntless.CharacterCard;
            DecisionSelectTarget = baron.CharacterCard;
            PlayCard(crystal);

            AssertNextToCard(crystal, dauntless.CharacterCard);
        }

        [Test()]
        public void TestPlayLocationOnRelic()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var arcshield = PlayCard("Arcshield");
            var crystal = PutInHand("Crystallization");

            DecisionSelectCard = arcshield;
            DecisionSelectTarget = baron.CharacterCard;
            PlayCard(crystal);

            AssertNextToCard(crystal, arcshield);
        }

        [Test()]
        public void TestEntryDealsDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var crystal = PutInHand("Crystallization");
            DecisionSelectCard = dauntless.CharacterCard;
            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            AssertDamageSource(dauntless.CharacterCard);
            AssertDamageType(DamageType.Energy);
            PlayCard(crystal);

            QuickHPCheck(-1);
        }

        [Test()]
        public void TestEntryDamageIsOptional()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var crystal = PutInHand("Crystallization");
            DecisionSelectCard = dauntless.CharacterCard;
            DecisionDoNotSelectCard = SelectionType.SelectTarget;

            QuickHPStorage(baron);
            PlayCard(crystal);

            QuickHPCheck(0); // No damage dealt
        }

        [Test()]
        public void TestCannotBeAffectedByVillainCards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var crystal = PutInHand("Crystallization");
            DecisionSelectCard = dauntless.CharacterCard;
            DecisionSelectTarget = baron.CharacterCard;
            PlayCard(crystal);
            AssertIsInPlay(crystal);

            // Verify Crystallization is not visible to villain card sources
            var villainCardSource = new CardSource(FindCardController(baron.CharacterCard));
            Assert.That(GameController.IsCardVisibleToCardSource(crystal, villainCardSource), Is.False,
                "Crystallization should not be visible to villain card sources");

            // Verify Crystallization IS visible to hero card sources (for contrast)
            var heroCardSource = new CardSource(FindCardController(dauntless.CharacterCard));
            Assert.That(GameController.IsCardVisibleToCardSource(crystal, heroCardSource), Is.True,
                "Crystallization should be visible to hero card sources");
        }

        [Test()]
        public void TestCannotBeAffectedByEnvironmentCards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var crystal = PutInHand("Crystallization");
            DecisionSelectCard = dauntless.CharacterCard;
            DecisionSelectTarget = baron.CharacterCard;
            PlayCard(crystal);
            AssertIsInPlay(crystal);

            // Get an environment card to use as source
            var envCard = PlayCard("VolcanicEruption");

            // Verify Crystallization is not visible to environment card sources
            var envCardSource = new CardSource(FindCardController(envCard));
            Assert.That(GameController.IsCardVisibleToCardSource(crystal, envCardSource), Is.False,
                "Crystallization should not be visible to environment card sources");

            // Verify Crystallization IS visible to hero card sources (for contrast)
            var heroCardSource = new CardSource(FindCardController(dauntless.CharacterCard));
            Assert.That(GameController.IsCardVisibleToCardSource(crystal, heroCardSource), Is.True,
                "Crystallization should be visible to hero card sources");
        }

        [Test()]
        public void TestReturnToHandWhenDestroyed()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Legacy", "InsulaPrimalis");
            StartGame();

            var crystal = PutInHand("Crystallization");
            DecisionSelectCard = dauntless.CharacterCard;
            DecisionSelectTarget = baron.CharacterCard;
            PlayCard(crystal);

            AssertIsInPlay(crystal);

            // Destroy with hero effect (e.g., Dauntless player)
            DestroyCard(crystal, dauntless.CharacterCard);

            // Should be in hand, not trash
            AssertInHand(crystal);
            AssertNotInTrash(crystal);
        }

        [Test()]
        public void TestMultipleCrystallizationsOnSameCard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var crystal1 = GetCard("Crystallization", 0);
            var crystal2 = GetCard("Crystallization", 1);

            PutInHand(crystal1);
            PutInHand(crystal2);

            DecisionSelectCard = dauntless.CharacterCard;
            DecisionSelectTarget = baron.CharacterCard;
            PlayCard(crystal1);
            PlayCard(crystal2);

            AssertNextToCard(crystal1, dauntless.CharacterCard);
            AssertNextToCard(crystal2, dauntless.CharacterCard);
        }

        [Test()]
        public void TestDestroyedWhenNextToCardLeavesPlay()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var arcshield = PlayCard("Arcshield");
            var crystal = PutInHand("Crystallization");

            DecisionSelectCard = arcshield;
            DecisionSelectTarget = baron.CharacterCard;
            PlayCard(crystal);

            AssertNextToCard(crystal, arcshield);

            // Destroy the Arcshield - Crystallization should also be destroyed (returned to hand)
            DestroyCard(arcshield, dauntless.CharacterCard);

            // Both should be in hand (return to hand effect)
            AssertInHand(arcshield);
            AssertInHand(crystal);
        }

        [Test()]
        public void TestCountsAsChargeForPowerScaling()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            // No charges - power deals 1
            QuickHPStorage(baron);
            DecisionSelectTarget = baron.CharacterCard;
            UsePower(dauntless.CharacterCard);
            QuickHPCheck(-1);

            // Add two Crystallizations to Dauntless
            DecisionSelectCard = dauntless.CharacterCard;
            PlayCard("Crystallization", 0);
            PlayCard("Crystallization", 1);

            // Two charges - power deals 2 (1 + 2/2)
            QuickHPStorage(baron);
            UsePower(dauntless.CharacterCard);
            QuickHPCheck(-2);
        }
    }
}
