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
    public class MatterToEnergyTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            DecisionSelectCard = dauntless.CharacterCard;
            var mte = PlayCard("MatterToEnergy");

            AssertNextToCard(mte, dauntless.CharacterCard);
        }

        [Test()]
        public void TestPlayLocationOnDauntless()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var mte = PutInHand("MatterToEnergy");
            DecisionSelectCard = dauntless.CharacterCard;
            PlayCard(mte);

            AssertNextToCard(mte, dauntless.CharacterCard);
        }

        [Test()]
        public void TestPlayLocationOnRelic()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var arcstep = PlayCard("Arcstep");
            var mte = PutInHand("MatterToEnergy");

            DecisionSelectCard = arcstep;
            PlayCard(mte);

            AssertNextToCard(mte, arcstep);
        }

        [Test()]
        public void TestCannotPlayOnCardWithMatterToEnergy()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            // Play one Matter To Energy on Dauntless
            var mte1 = GetCard("MatterToEnergy", 0);
            PutInHand(mte1);
            DecisionSelectCard = dauntless.CharacterCard;
            PlayCard(mte1);

            AssertNextToCard(mte1, dauntless.CharacterCard);

            // Play Arcstep as alternative location
            var arcstep = PlayCard("Arcstep");

            // Try to play second MTE - should only offer Arcstep
            var mte2 = GetCard("MatterToEnergy", 1);
            PutInHand(mte2);
            DecisionSelectCard = arcstep; // Only valid option is Arcstep

            PlayCard(mte2);

            AssertNextToCard(mte2, arcstep);
        }

        [Test()]
        public void TestCountsAsCharge()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            // Play Matter To Energy on Dauntless - counts as a charge
            DecisionSelectCard = dauntless.CharacterCard;
            PlayCard("MatterToEnergy");

            // 1 charge = 1 + 1/2 = 1 damage still
            QuickHPStorage(baron);
            DecisionSelectTarget = baron.CharacterCard;
            DecisionSelectDamageType = DamageType.Energy;
            UsePower(dauntless.CharacterCard);
            QuickHPCheck(-1);

            // Add another charge
            DecisionSelectCard = dauntless.CharacterCard;
            PlayCard("Crystallization");

            // 2 charges = 1 + 2/2 = 2 damage
            QuickHPStorage(baron);
            UsePower(dauntless.CharacterCard);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestEffectOnDauntless()
        {
            // On Dauntless: Choose damage type for power
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            DecisionSelectCard = dauntless.CharacterCard;
            PlayCard("MatterToEnergy");

            DecisionSelectTarget = baron.CharacterCard;
            DecisionSelectDamageType = DamageType.Fire;
            AssertDamageType(DamageType.Fire);

            UsePower(dauntless.CharacterCard);
        }

        [Test()]
        public void TestEffectOnDauntlessChooseCold()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            DecisionSelectCard = dauntless.CharacterCard;
            PlayCard("MatterToEnergy");

            DecisionSelectTarget = baron.CharacterCard;
            DecisionSelectDamageType = DamageType.Cold;
            AssertDamageType(DamageType.Cold);

            UsePower(dauntless.CharacterCard);
        }

        [Test()]
        public void TestEffectOnArcshield()
        {
            // On Arcshield: After power, reduce chosen damage type by 1
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var arcshield = PlayCard("Arcshield");
            DecisionSelectCard = arcshield;
            PlayCard("MatterToEnergy");

            DecisionSelectDamageType = DamageType.Fire;
            UsePower(arcshield);

            // Fire damage should be reduced by 1 (from MTE) + Arcshield's reduction to Dauntless
            QuickHPStorage(dauntless);
            DealDamage(baron, dauntless, 3, DamageType.Fire);
            QuickHPCheck(-1); // 3 - 1 (Arcshield) - 1 (MTE Fire) = 1

            // Melee damage only reduced by Arcshield
            QuickHPStorage(dauntless);
            DealDamage(baron, dauntless, 3, DamageType.Melee);
            QuickHPCheck(-2); // 3 - 1 (Arcshield) = 2
        }

        [Test()]
        public void TestEffectOnArcstep()
        {
            // On Arcstep: After power, deal 1 damage to up to 3 targets
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
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
        public void TestDestroyedWhenNextToCardLeavesPlay()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var arcstep = PlayCard("Arcstep");
            DecisionSelectCard = arcstep;
            var mte = PlayCard("MatterToEnergy");

            AssertNextToCard(mte, arcstep);

            // Destroy Arcstep - MTE should be destroyed too
            DestroyCard(arcstep, dauntless.CharacterCard);

            AssertInHand(arcstep); // Returned to hand
            AssertInTrash(mte); // Destroyed normally (no return to hand for MTE)
        }

        [Test()]
        public void TestCanHaveBothPlasmaCoreAndMatterToEnergyOnSameCard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            DecisionSelectCard = dauntless.CharacterCard;
            var plasmaCore = PlayCard("PlasmaCore");
            var mte = PlayCard("MatterToEnergy");

            AssertNextToCard(plasmaCore, dauntless.CharacterCard);
            AssertNextToCard(mte, dauntless.CharacterCard);
        }
    }
}
