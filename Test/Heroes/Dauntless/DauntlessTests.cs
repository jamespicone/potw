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
    public class DauntlessTests : ParahumanTest
    {
        [Test()]
        public void TestArclancePowerBaseDamage()
        {
            // Base damage is 1 (no charges)
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            QuickHPStorage(baron);
            DecisionSelectTarget = baron.CharacterCard;
            AssertDamageSource(dauntless.CharacterCard);
            AssertDamageType(DamageType.Energy);

            UsePower(dauntless.CharacterCard);

            QuickHPCheck(-1);
        }

        [Test()]
        public void TestArclancePowerWithOneCharge()
        {
            // 1 charge = 1 + 1/2 = 1 damage
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            // Attach one Crystallization to Dauntless
            DecisionSelectCard = dauntless.CharacterCard;
            PlayCard("Crystallization");

            QuickHPStorage(baron);
            DecisionSelectTarget = baron.CharacterCard;

            UsePower(dauntless.CharacterCard);

            QuickHPCheck(-1); // 1 + 1/2 = 1
        }

        [Test()]
        public void TestArclancePowerWithTwoCharges()
        {
            // 2 charges = 1 + 2/2 = 2 damage
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            // Attach two charges to Dauntless
            DecisionSelectCard = dauntless.CharacterCard;
            PlayCard("Crystallization", 0);
            PlayCard("Crystallization", 1);

            QuickHPStorage(baron);
            DecisionSelectTarget = baron.CharacterCard;

            UsePower(dauntless.CharacterCard);

            QuickHPCheck(-2); // 1 + 2/2 = 2
        }

        [Test()]
        public void TestArclancePowerWithThreeCharges()
        {
            // 3 charges = 1 + 3/2 = 2 damage (rounds down)
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            // Attach three charges to Dauntless
            DecisionSelectCard = dauntless.CharacterCard;
            PlayCard("Crystallization", 0);
            PlayCard("Crystallization", 1);
            PlayCard("PlasmaCore");

            QuickHPStorage(baron);
            DecisionSelectTarget = baron.CharacterCard;

            UsePower(dauntless.CharacterCard);

            QuickHPCheck(-2); // 1 + 3/2 = 2
        }

        [Test()]
        public void TestArclancePowerWithFourCharges()
        {
            // 4 charges = 1 + 4/2 = 3 damage
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            // Attach four charges to Dauntless
            DecisionSelectCard = dauntless.CharacterCard;
            PlayCard("Crystallization", 0);
            PlayCard("Crystallization", 1);
            PlayCard("PlasmaCore");
            PlayCard("MatterToEnergy");

            QuickHPStorage(baron);
            DecisionSelectTarget = baron.CharacterCard;
            DecisionSelectDamageType = DamageType.Energy;

            UsePower(dauntless.CharacterCard);

            QuickHPCheck(-3); // 1 + 4/2 = 3
        }

        [Test()]
        public void TestArclancePowerOnlyCountsChargesOnDauntless()
        {
            // Charges on relics don't count for Dauntless power
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            // Put Arcshield into play and attach charges to it
            var arcshield = PlayCard("Arcshield");
            DecisionSelectCard = arcshield;
            PlayCard("Crystallization", 0);
            PlayCard("Crystallization", 1);

            QuickHPStorage(baron);
            DecisionSelectTarget = baron.CharacterCard;

            UsePower(dauntless.CharacterCard);

            QuickHPCheck(-1); // Base damage only, charges on Arcshield don't count
        }

        [Test()]
        public void TestArclancePowerWithPlasmaCore()
        {
            // Plasma Core on Dauntless makes damage irreducible
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Legacy", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            // Play Plasma Core on Dauntless
            DecisionSelectCard = dauntless.CharacterCard;
            PlayCard("PlasmaCore");

            // Play Heroic Interception to reduce damage
            PlayCard("HeroicInterception");

            QuickHPStorage(baron);
            DecisionSelectTarget = baron.CharacterCard;
            AssertIrreducible();

            UsePower(dauntless.CharacterCard);

            QuickHPCheck(-1); // Irreducible, ignores damage reduction
        }

        [Test()]
        public void TestArclancePowerWithMatterToEnergy()
        {
            // Matter to Energy on Dauntless lets you choose damage type
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            // Play Matter To Energy on Dauntless
            DecisionSelectCard = dauntless.CharacterCard;
            PlayCard("MatterToEnergy");

            QuickHPStorage(baron);
            DecisionSelectTarget = baron.CharacterCard;
            DecisionSelectDamageType = DamageType.Fire;
            AssertDamageType(DamageType.Fire);

            UsePower(dauntless.CharacterCard);

            QuickHPCheck(-1);
        }

        [Test()]
        public void TestArclancePowerWithMatterToEnergyChooseLightning()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            // Play Matter To Energy on Dauntless
            DecisionSelectCard = dauntless.CharacterCard;
            PlayCard("MatterToEnergy");

            QuickHPStorage(baron);
            DecisionSelectTarget = baron.CharacterCard;
            DecisionSelectDamageType = DamageType.Lightning;
            AssertDamageType(DamageType.Lightning);

            UsePower(dauntless.CharacterCard);

            QuickHPCheck(-1);
        }

        [Test()]
        public void TestIncapFirstAbilityDraw()
        {
            // One player may draw a card
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Legacy", "InsulaPrimalis");
            StartGame();

            IncapacitateCharacter(dauntless.CharacterCard, baron.CharacterCard);
            AssertIncapacitated(dauntless);

            QuickHandStorage(legacy);
            DecisionSelectTurnTaker = legacy.TurnTaker;

            UseIncapacitatedAbility(dauntless, 0);

            QuickHandCheck(1);
        }

        [Test()]
        public void TestIncapSecondAbilityReducesEnergyDamage()
        {
            // Reduce all energy damage dealt to hero targets by 1 until next turn
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            IncapacitateCharacter(dauntless.CharacterCard, baron.CharacterCard);
            AssertIncapacitated(dauntless);

            UseIncapacitatedAbility(dauntless, 1);

            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Energy);
            QuickHPCheck(-2); // Reduced by 1

            // Fire damage is not reduced
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Fire);
            QuickHPCheck(-3); // Full damage
        }

        [Test()]
        public void TestIncapSecondAbilityExpiresAtNextTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            IncapacitateCharacter(dauntless.CharacterCard, baron.CharacterCard);
            AssertIncapacitated(dauntless);

            UseIncapacitatedAbility(dauntless, 1);

            // Go to start of Dauntless's next turn
            GoToStartOfTurn(dauntless);

            // Damage reduction should be gone
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Energy);
            QuickHPCheck(-3); // Full damage now
        }

        [Test()]
        public void TestIncapThirdAbilityMakesEquipmentIndestructible()
        {
            // Select an equipment card. Until your next turn it is indestructible
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            // Play an equipment card
            var flakCannon = PlayCard("FlakCannon");

            IncapacitateCharacter(dauntless.CharacterCard, baron.CharacterCard);
            AssertIncapacitated(dauntless);

            DecisionSelectCard = flakCannon;
            UseIncapacitatedAbility(dauntless, 2);

            // Try to destroy the equipment - should fail
            DestroyCard(flakCannon);
            AssertIsInPlay(flakCannon);
        }

        [Test()]
        public void TestIncapThirdAbilityIndestructibleExpiresAtNextTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            var flakCannon = PlayCard("FlakCannon");

            IncapacitateCharacter(dauntless.CharacterCard, baron.CharacterCard);
            AssertIncapacitated(dauntless);

            DecisionSelectCard = flakCannon;
            UseIncapacitatedAbility(dauntless, 2);

            // Go to start of Dauntless's next turn
            GoToStartOfTurn(dauntless);

            // Equipment should be destructible now
            DestroyCard(flakCannon);
            AssertInTrash(flakCannon);
        }
    }
}
