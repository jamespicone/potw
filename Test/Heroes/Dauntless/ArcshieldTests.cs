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
    public class ArcshieldTests : ParahumanTest
    {
        [Test()]
        public void TestPowerBaseReduction()
        {
            // Base reduction is 1 (no charges)
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var arcshield = PlayCard("Arcshield");
            UsePower(arcshield);

            QuickHPStorage(dauntless);
            DealDamage(baron, dauntless, 3, DamageType.Melee);
            QuickHPCheck(-2); // 3 - 1 = 2
        }

        [Test()]
        public void TestPowerWithOneCharge()
        {
            // 1 charge = 1 + 1/2 = 1 reduction
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var arcshield = PlayCard("Arcshield");
            DecisionSelectCard = arcshield;
            PlayCard("Crystallization");

            UsePower(arcshield);

            QuickHPStorage(dauntless);
            DealDamage(baron, dauntless, 3, DamageType.Melee);
            QuickHPCheck(-2); // 3 - 1 = 2
        }

        [Test()]
        public void TestPowerWithTwoCharges()
        {
            // 2 charges = 1 + 2/2 = 2 reduction
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var arcshield = PlayCard("Arcshield");
            DecisionSelectCard = arcshield;
            DecisionSelectTarget = baron.CharacterCard;
            PlayCard("Crystallization", 0);
            PlayCard("Crystallization", 1);

            UsePower(arcshield);

            QuickHPStorage(dauntless);
            DealDamage(baron, dauntless, 3, DamageType.Melee);
            QuickHPCheck(-1); // 3 - 2 = 1
        }

        [Test()]
        public void TestPowerWithFourCharges()
        {
            // 4 charges = 1 + 4/2 = 3 reduction
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var arcshield = PlayCard("Arcshield");
            // Use Plasma Core + Matter to Energy + 2 Crystallizations = 4 charges
            DecisionSelectCard = arcshield;
            DecisionSelectTarget = baron.CharacterCard;
            PlayCard("PlasmaCore");
            PlayCard("MatterToEnergy");
            PlayCard("Crystallization", 0);
            PlayCard("Crystallization", 1);

            DecisionSelectDamageType = DamageType.Energy;
            UsePower(arcshield);

            QuickHPStorage(dauntless);
            DealDamage(baron, dauntless, 5, DamageType.Melee);
            QuickHPCheck(-2); // 5 - 3 = 2
        }

        [Test()]
        public void TestPowerOnlyReducesDamageToDauntless()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            var arcshield = PlayCard("Arcshield");
            UsePower(arcshield);

            // Damage to Dauntless is reduced
            QuickHPStorage(dauntless);
            DealDamage(baron, dauntless, 3, DamageType.Melee);
            QuickHPCheck(-2); // Reduced by 1

            // Damage to Bunker is not reduced
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Melee);
            QuickHPCheck(-3); // Full damage
        }

        [Test()]
        public void TestPowerExpiresAtEndOfNextTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var arcshield = PlayCard("Arcshield");
            UsePower(arcshield);

            // Go to end of next turn
            // This works because we're not on dauntless' turn - we're on Baron Blade's turn,
            // so this will be the end of our next turn.
            GoToEndOfTurn(dauntless);

            // Damage reduction should be gone
            QuickHPStorage(dauntless);
            DealDamage(baron, dauntless, 3, DamageType.Melee);
            QuickHPCheck(-3); // Full damage
        }

        [Test()]
        public void TestPowerExpiresAtEndOfNextTurnWhenUsedOnOwnTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            // Go to Dauntless' turn and use the power there
            GoToUsePowerPhase(dauntless);

            var arcshield = PlayCard("Arcshield");
            UsePower(arcshield);

            // Reduction should still work during this turn
            QuickHPStorage(dauntless);
            DealDamage(baron, dauntless, 3, DamageType.Melee);
            QuickHPCheck(-2); // Reduced by 1

            // Go through a full round back to Dauntless
            GoToStartOfTurn(dauntless);

            // Reduction should still work (expires at END of next turn)
            QuickHPStorage(dauntless);
            DealDamage(baron, dauntless, 3, DamageType.Melee);
            QuickHPCheck(-2); // Still reduced

            // Now go to end of this turn (the "next" turn)
            GoToEndOfTurn(dauntless);

            // Reduction should now be gone
            QuickHPStorage(dauntless);
            DealDamage(baron, dauntless, 3, DamageType.Melee);
            QuickHPCheck(-3); // Full damage
        }

        [Test()]
        public void TestCannotBeAffectedByVillainCards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var arcshield = PlayCard("Arcshield");
            AssertIsInPlay(arcshield);

            // Verify Arcshield is not visible to villain card sources
            var villainCardSource = new CardSource(FindCardController(baron.CharacterCard));
            Assert.That(GameController.IsCardVisibleToCardSource(arcshield, villainCardSource), Is.False,
                "Arcshield should not be visible to villain card sources");

            // Verify Arcshield IS visible to hero card sources (for contrast)
            var heroCardSource = new CardSource(FindCardController(dauntless.CharacterCard));
            Assert.That(GameController.IsCardVisibleToCardSource(arcshield, heroCardSource), Is.True,
                "Arcshield should be visible to hero card sources");
        }

        [Test()]
        public void TestCannotBeAffectedByEnvironmentCards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var arcshield = PlayCard("Arcshield");
            AssertIsInPlay(arcshield);

            // Get an environment card to use as source
            var envCard = PlayCard("VolcanicEruption");

            // Verify Arcshield is not visible to environment card sources
            var envCardSource = new CardSource(FindCardController(envCard));
            Assert.That(GameController.IsCardVisibleToCardSource(arcshield, envCardSource), Is.False,
                "Arcshield should not be visible to environment card sources");

            // Verify Arcshield IS visible to hero card sources (for contrast)
            var heroCardSource = new CardSource(FindCardController(dauntless.CharacterCard));
            Assert.That(GameController.IsCardVisibleToCardSource(arcshield, heroCardSource), Is.True,
                "Arcshield should be visible to hero card sources");
        }

        [Test()]
        public void TestReturnToHandWhenDestroyed()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Legacy", "InsulaPrimalis");
            StartGame();

            var arcshield = PlayCard("Arcshield");
            AssertIsInPlay(arcshield);

            // Destroy with hero effect
            DestroyCard(arcshield, dauntless.CharacterCard);

            // Should be in hand, not trash
            AssertInHand(arcshield);
            AssertNotInTrash(arcshield);
        }

        [Test()]
        public void TestPlasmaCoreCounterDamage()
        {
            // Plasma Core on Arcshield: Whenever damage is reduced by Arcshield's power, Dauntless deals the source 1 energy damage
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var arcshield = PlayCard("Arcshield");
            DecisionSelectCard = arcshield;
            PlayCard("PlasmaCore");

            UsePower(arcshield);

            QuickHPStorage(dauntless, baron);

            // Deal damage to Dauntless - should trigger counter damage
            DealDamage(baron, dauntless, 3, DamageType.Melee);

            // Dauntless takes 2 (reduced by 1), Baron takes 1 counter damage
            QuickHPCheck(-2, -1);
        }

        [Test()]
        public void TestPlasmaCoreCounterDamageSource()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var arcshield = PlayCard("Arcshield");
            DecisionSelectCard = arcshield;
            PlayCard("PlasmaCore");

            UsePower(arcshield);

            QuickHPStorage(baron);

            // Assert that the counter damage comes from Dauntless and is Energy type
            AssertDamageSource(dauntless.CharacterCard);
            AssertDamageType(DamageType.Energy);
            DealDamage(baron, dauntless, 3, DamageType.Melee);

            // Verify Baron took counter damage
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestMatterToEnergyAdditionalReduction()
        {
            // Matter to Energy on Arcshield: After using power, pick a damage type; reduce all damage of that type by 1
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var arcshield = PlayCard("Arcshield");
            DecisionSelectCard = arcshield;
            PlayCard("MatterToEnergy");

            DecisionSelectDamageType = DamageType.Fire;
            UsePower(arcshield);

            // Fire damage should be reduced by 1 (additional effect from Matter To Energy)
            QuickHPStorage(dauntless);
            DealDamage(baron, dauntless, 3, DamageType.Fire);
            QuickHPCheck(-1); // 3 - 1 (Arcshield) - 1 (MTE fire reduction) = 1

            // Melee damage only reduced by Arcshield
            QuickHPStorage(dauntless);
            DealDamage(baron, dauntless, 3, DamageType.Melee);
            QuickHPCheck(-2); // 3 - 1 (Arcshield) = 2
        }

        [Test()]
        public void TestMatterToEnergyReductionExpiresAtEndOfNextTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var arcshield = PlayCard("Arcshield");
            DecisionSelectCard = arcshield;
            PlayCard("MatterToEnergy");

            DecisionSelectDamageType = DamageType.Fire;
            UsePower(arcshield);

            // Go to end of next turn
            GoToEndOfTurn(dauntless);

            // Both reductions should be gone
            QuickHPStorage(dauntless);
            DealDamage(baron, dauntless, 3, DamageType.Fire);
            QuickHPCheck(-3); // Full damage
        }

        [Test()]
        public void TestMatterToEnergyReductionExpiresAtEndOfNextTurnWhenUsedOnOwnTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            // Go to Dauntless' turn and use the power there
            GoToUsePowerPhase(dauntless);

            var arcshield = PlayCard("Arcshield");
            DecisionSelectCard = arcshield;
            PlayCard("MatterToEnergy");

            DecisionSelectDamageType = DamageType.Fire;
            UsePower(arcshield);

            // Fire damage should be reduced by 2 (Arcshield + MTE) during this turn
            QuickHPStorage(dauntless);
            DealDamage(baron, dauntless, 3, DamageType.Fire);
            QuickHPCheck(-1); // 3 - 1 - 1 = 1

            // Go through a full round back to Dauntless
            GoToStartOfTurn(dauntless);

            // Reduction should still work (expires at END of next turn)
            QuickHPStorage(dauntless);
            DealDamage(baron, dauntless, 3, DamageType.Fire);
            QuickHPCheck(-1); // Still reduced

            // Now go to end of this turn (the "next" turn)
            GoToEndOfTurn(dauntless);

            // Both reductions should now be gone
            QuickHPStorage(dauntless);
            DealDamage(baron, dauntless, 3, DamageType.Fire);
            QuickHPCheck(-3); // Full damage
        }

        [Test()]
        public void TestChargesOnArcshieldCountForArcshieldOnly()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var arcshield = PlayCard("Arcshield");

            // Put charges on Arcshield
            DecisionSelectCard = arcshield;
            DecisionSelectTarget = baron.CharacterCard;
            PlayCard("Crystallization", 0);
            PlayCard("Crystallization", 1);

            // Arcshield power should scale with its charges
            UsePower(arcshield);

            QuickHPStorage(dauntless);
            DealDamage(baron, dauntless, 3, DamageType.Melee);
            QuickHPCheck(-1); // 3 - 2 = 1

            // But Dauntless power should not scale (charges on Arcshield, not Dauntless)
            QuickHPStorage(baron);
            DecisionSelectTarget = baron.CharacterCard;
            UsePower(dauntless.CharacterCard);
            QuickHPCheck(-1); // Base damage only
        }

        [Test()]
        public void TestPlasmaCoreAndMatterToEnergySynergy()
        {
            // Test that MTE reduction triggers Plasma Core counter damage, even on damage to other heroes
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var arcshield = PlayCard("Arcshield");
            DecisionSelectCard = arcshield;
            PlayCard("PlasmaCore");
            PlayCard("MatterToEnergy");

            // MTE reduces ALL damage of selected type, not just to Dauntless
            DecisionSelectDamageType = DamageType.Melee;
            UsePower(arcshield);

            QuickHPStorage(bunker, baron);

            // Deal melee damage to Bunker - MTE reduces by 1, and Plasma Core should trigger
            DealDamage(baron, bunker, 3, DamageType.Melee);

            // Bunker takes 2 (3 - 1 from MTE), Baron takes 1 counter damage from Plasma Core
            QuickHPCheck(-2, -1);
        }

        [Test()]
        public void TestPlasmaCoreNoRetaliationIfDamageFullyPrevented()
        {
            // Plasma Core counter damage should NOT trigger if damage is fully prevented to 0
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var arcshield = PlayCard("Arcshield");
            DecisionSelectCard = arcshield;
            PlayCard("PlasmaCore");

            UsePower(arcshield);

            QuickHPStorage(dauntless, baron);

            // Deal only 1 damage - Arcshield reduces by 1, so no damage dealt
            DealDamage(baron, dauntless, 1, DamageType.Melee);

            // Neither should take damage - reduction fully prevented damage, no counter trigger
            QuickHPCheck(0, 0);
        }
    }
}
