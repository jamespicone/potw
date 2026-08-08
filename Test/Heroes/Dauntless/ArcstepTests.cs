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
    public class ArcstepTests : ParahumanTest
    {
        [Test()]
        public void TestPowerBaseDrawAndPlay()
        {
            // Base draw is 1 (no charges), plus optional play
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var arcstep = PlayCard("Arcstep");

            QuickHandStorage(dauntless);
            DecisionDoNotSelectCard = SelectionType.PlayCard; // Skip the optional play
            UsePower(arcstep);

            QuickHandCheck(1); // Drew 1 card
        }

        [Test()]
        public void TestPowerWithOneCharge()
        {
            // 1 charge = 1 + 1/2 = 1 draw
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var arcstep = PlayCard("Arcstep");
            DecisionSelectCard = arcstep;
            DecisionSelectTarget = baron.CharacterCard;
            PlayCard("Crystallization");

            QuickHandStorage(dauntless);
            DecisionDoNotSelectCard = SelectionType.PlayCard;
            UsePower(arcstep);

            QuickHandCheck(1); // Still 1 draw with 1 charge
        }

        [Test()]
        public void TestPowerWithTwoCharges()
        {
            // 2 charges = 1 + 2/2 = 2 draws
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var arcstep = PlayCard("Arcstep");
            DecisionSelectCard = arcstep;
            DecisionSelectTarget = baron.CharacterCard;
            PlayCard("Crystallization", 0);
            PlayCard("Crystallization", 1);

            QuickHandStorage(dauntless);
            DecisionDoNotSelectCard = SelectionType.PlayCard;
            UsePower(arcstep);

            QuickHandCheck(2); // 2 draws with 2 charges
        }

        [Test()]
        public void TestPowerWithFourCharges()
        {
            // 4 charges = 1 + 4/2 = 3 draws
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var arcstep = PlayCard("Arcstep");
            // Use Plasma Core + Matter to Energy + 2 Crystallizations = 4 charges
            DecisionSelectCard = arcstep;
            DecisionSelectTarget = baron.CharacterCard;
            PlayCard("PlasmaCore");
            PlayCard("MatterToEnergy");
            PlayCard("Crystallization", 0);
            PlayCard("Crystallization", 1);

            QuickHandStorage(dauntless);
            DecisionDoNotSelectCard = SelectionType.PlayCard;
            DecisionSelectTargets = new Card[] { null, null, null };
            UsePower(arcstep);

            QuickHandCheck(3); // 3 draws with 4 charges
        }

        [Test()]
        public void TestPowerAllowsOptionalPlay()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var arcstep = PlayCard("Arcstep");

            // Put a card in hand to play
            var corona = PutInHand("CracklingCorona");

            // The draw will add another card, then we can play
            DecisionSelectCardToPlay = corona;
            UsePower(arcstep);

            // Corona should now be in trash (one-shot)
            AssertInTrash(corona);
        }

        [Test()]
        public void TestCannotBeAffectedByVillainCards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var arcstep = PlayCard("Arcstep");
            AssertIsInPlay(arcstep);

            // Verify Arcstep is not visible to villain card sources
            var villainCardSource = new CardSource(FindCardController(baron.CharacterCard));
            Assert.That(GameController.IsCardVisibleToCardSource(arcstep, villainCardSource), Is.False,
                "Arcstep should not be visible to villain card sources");

            // Verify Arcstep IS visible to hero card sources (for contrast)
            var heroCardSource = new CardSource(FindCardController(dauntless.CharacterCard));
            Assert.That(GameController.IsCardVisibleToCardSource(arcstep, heroCardSource), Is.True,
                "Arcstep should be visible to hero card sources");
        }

        [Test()]
        public void TestCannotBeAffectedByEnvironmentCards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var arcstep = PlayCard("Arcstep");
            AssertIsInPlay(arcstep);

            // Get an environment card to use as source
            var envCard = PlayCard("VolcanicEruption");

            // Verify Arcstep is not visible to environment card sources
            var envCardSource = new CardSource(FindCardController(envCard));
            Assert.That(GameController.IsCardVisibleToCardSource(arcstep, envCardSource), Is.False,
                "Arcstep should not be visible to environment card sources");

            // Verify Arcstep IS visible to hero card sources (for contrast)
            var heroCardSource = new CardSource(FindCardController(dauntless.CharacterCard));
            Assert.That(GameController.IsCardVisibleToCardSource(arcstep, heroCardSource), Is.True,
                "Arcstep should be visible to hero card sources");
        }

        [Test()]
        public void TestReturnToHandWhenDestroyed()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Legacy", "InsulaPrimalis");
            StartGame();

            var arcstep = PlayCard("Arcstep");
            AssertIsInPlay(arcstep);

            // Destroy with hero effect
            DestroyCard(arcstep, dauntless.CharacterCard);

            // Should be in hand, not trash
            AssertInHand(arcstep);
            AssertNotInTrash(arcstep);
        }

        [Test()]
        public void TestPlasmaCoreHealsBeforePower()
        {
            // Plasma Core on Arcstep: Before using power, regain 1 HP
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var arcstep = PlayCard("Arcstep");
            DecisionSelectCard = arcstep;
            PlayCard("PlasmaCore");

            // Damage Dauntless first
            SetHitPoints(dauntless, 20);

            QuickHPStorage(dauntless);
            DecisionDoNotSelectCard = SelectionType.PlayCard;
            UsePower(arcstep);

            QuickHPCheck(1); // Gained 1 HP
        }

        [Test()]
        public void TestPlasmaCoreHealsEvenIfAtMax()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var arcstep = PlayCard("Arcstep");
            DecisionSelectCard = arcstep;
            PlayCard("PlasmaCore");

            // Dauntless at full HP
            QuickHPStorage(dauntless);
            DecisionDoNotSelectCard = SelectionType.PlayCard;
            UsePower(arcstep);

            QuickHPCheck(0); // Can't heal past max
        }

        [Test()]
        public void TestMatterToEnergyDealsDamageAfterPower()
        {
            // Matter to Energy on Arcstep: After using power, deal up to 3 targets 1 energy damage
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Legacy", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var arcstep = PlayCard("Arcstep");
            DecisionSelectCard = arcstep;
            PlayCard("MatterToEnergy");

            QuickHPStorage(baron);
            DecisionDoNotSelectCard = SelectionType.PlayCard;
            DecisionSelectTargets = new Card[] { baron.CharacterCard, null, null };
            AssertDamageSource(dauntless.CharacterCard);
            AssertDamageType(DamageType.Energy);

            UsePower(arcstep);

            QuickHPCheck(-1); // 1 damage to Baron
        }

        [Test()]
        public void TestMatterToEnergyCanTargetMultiple()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            // Add multiple villain targets that don't have immunity
            var battalion = PlayCard("BladeBattalion");

            var arcstep = PlayCard("Arcstep");
            DecisionSelectCard = arcstep;
            PlayCard("MatterToEnergy");

            QuickHPStorage(baron, bunker);
            var battalionStartHP = battalion.HitPoints;
            DecisionDoNotSelectCard = SelectionType.PlayCard;
            DecisionSelectTargets = new Card[] { baron.CharacterCard, battalion, bunker.CharacterCard };

            UsePower(arcstep);

            QuickHPCheck(-1, -1); // 1 damage to baron and bunker
            Assert.That(battalion.HitPoints, Is.EqualTo(battalionStartHP - 1)); // 1 damage to Battalion
        }

        [Test()]
        public void TestMatterToEnergyDamageIsOptional()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var arcstep = PlayCard("Arcstep");
            DecisionSelectCard = arcstep;
            PlayCard("MatterToEnergy");

            QuickHPStorage(baron);
            DecisionDoNotSelectCard = SelectionType.PlayCard;
            DecisionSelectTargets = new Card[] { null, null, null };

            UsePower(arcstep);

            QuickHPCheck(0); // No targets selected
        }

        [Test()]
        public void TestChargesOnArcstepCountForArcstepOnly()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var arcstep = PlayCard("Arcstep");

            // Put charges on Arcstep
            DecisionSelectCard = arcstep;
            DecisionSelectTarget = baron.CharacterCard;
            PlayCard("Crystallization", 0);
            PlayCard("Crystallization", 1);

            // Arcstep power should scale with its charges
            QuickHandStorage(dauntless);
            DecisionDoNotSelectCard = SelectionType.PlayCard;
            UsePower(arcstep);
            QuickHandCheck(2); // 2 draws with 2 charges

            // But Dauntless power should not scale (charges on Arcstep, not Dauntless)
            QuickHPStorage(baron);
            DecisionSelectTarget = baron.CharacterCard;
            UsePower(dauntless.CharacterCard);
            QuickHPCheck(-1); // Base damage only
        }

        [Test()]
        public void TestBothPlasmaCoreAndMatterToEnergy()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var arcstep = PlayCard("Arcstep");
            DecisionSelectCard = arcstep;
            PlayCard("PlasmaCore");
            PlayCard("MatterToEnergy");

            // Damage Dauntless
            SetHitPoints(dauntless, 20);

            QuickHPStorage(dauntless, baron);
            DecisionDoNotSelectCard = SelectionType.PlayCard;
            DecisionSelectTargets = new Card[] { baron.CharacterCard, null, null };

            UsePower(arcstep);

            // Dauntless gains 1 HP, Baron takes 1 damage
            QuickHPCheck(1, -1);
        }
    }
}
