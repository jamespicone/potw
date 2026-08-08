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
    public class PlasmaCoreTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            DecisionSelectCard = dauntless.CharacterCard;
            var plasmaCore = PlayCard("PlasmaCore");

            AssertNextToCard(plasmaCore, dauntless.CharacterCard);
        }

        [Test()]
        public void TestPlayLocationOnDauntless()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var plasmaCore = PutInHand("PlasmaCore");
            DecisionSelectCard = dauntless.CharacterCard;
            PlayCard(plasmaCore);

            AssertNextToCard(plasmaCore, dauntless.CharacterCard);
        }

        [Test()]
        public void TestPlayLocationOnRelic()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var arcshield = PlayCard("Arcshield");
            var plasmaCore = PutInHand("PlasmaCore");

            DecisionSelectCard = arcshield;
            PlayCard(plasmaCore);

            AssertNextToCard(plasmaCore, arcshield);
        }

        [Test()]
        public void TestCannotPlayOnCardWithPlasmaCore()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            // Play one Plasma Core on Dauntless
            var plasmaCore1 = GetCard("PlasmaCore", 0);
            PutInHand(plasmaCore1);
            DecisionSelectCard = dauntless.CharacterCard;
            PlayCard(plasmaCore1);

            AssertNextToCard(plasmaCore1, dauntless.CharacterCard);

            // Play Arcshield as alternative location
            var arcshield = PlayCard("Arcshield");

            // Try to play second Plasma Core - should only offer Arcshield
            var plasmaCore2 = GetCard("PlasmaCore", 1);
            PutInHand(plasmaCore2);
            DecisionSelectCard = arcshield; // Only valid option is Arcshield

            PlayCard(plasmaCore2);

            AssertNextToCard(plasmaCore2, arcshield);
        }

        [Test()]
        public void TestCountsAsCharge()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            // Play Plasma Core on Dauntless - counts as a charge
            DecisionSelectCard = dauntless.CharacterCard;
            PlayCard("PlasmaCore");

            // 1 charge = 1 + 1/2 = 1 damage still
            QuickHPStorage(baron);
            DecisionSelectTarget = baron.CharacterCard;
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
            // On Dauntless: Power does irreducible damage
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Legacy", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            DecisionSelectCard = dauntless.CharacterCard;
            PlayCard("PlasmaCore");

            // Play a damage reducer
            PlayCard("HeroicInterception");

            QuickHPStorage(baron);
            DecisionSelectTarget = baron.CharacterCard;
            AssertIrreducible();

            UsePower(dauntless.CharacterCard);

            QuickHPCheck(-1); // Irreducible, ignores damage reduction
        }

        [Test()]
        public void TestEffectOnArcshield()
        {
            // On Arcshield: Counter damage when damage is reduced
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var arcshield = PlayCard("Arcshield");
            DecisionSelectCard = arcshield;
            PlayCard("PlasmaCore");

            // Use Arcshield's power to activate damage reduction
            UsePower(arcshield);

            // Deal damage to Dauntless - should trigger counter damage
            QuickHPStorage(baron);
            DealDamage(baron, dauntless, 3, DamageType.Melee);

            QuickHPCheck(-1); // Baron takes 1 counter damage
        }

        [Test()]
        public void TestEffectOnArcstep()
        {
            // On Arcstep: Regain 1 HP before using power
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var arcstep = PlayCard("Arcstep");
            DecisionSelectCard = arcstep;
            PlayCard("PlasmaCore");

            SetHitPoints(dauntless, 20);

            QuickHPStorage(dauntless);
            DecisionDoNotSelectCard = SelectionType.PlayCard;
            UsePower(arcstep);

            QuickHPCheck(1); // Gained 1 HP
        }

        [Test()]
        public void TestDestroyedWhenNextToCardLeavesPlay()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var arcshield = PlayCard("Arcshield");
            DecisionSelectCard = arcshield;
            var plasmaCore = PlayCard("PlasmaCore");

            AssertNextToCard(plasmaCore, arcshield);

            // Destroy Arcshield - Plasma Core should be destroyed too
            DestroyCard(arcshield, dauntless.CharacterCard);

            AssertInHand(arcshield); // Returned to hand
            AssertInTrash(plasmaCore); // Destroyed normally (no return to hand for Plasma Core)
        }
    }
}
