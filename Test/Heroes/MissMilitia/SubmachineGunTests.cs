using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.MissMilitia
{
    [TestFixture()]
    public class SubmachineGunTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "InsulaPrimalis");
        }

        [Test()]
        public void TestIsWeaponEquipment()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "InsulaPrimalis");
            StartGame();

            var card = GetCard("SubmachineGun");
            Assert.That(card.DoKeywordsContain("weapon"), Is.True);
            Assert.That(card.DoKeywordsContain("equipment"), Is.True);
        }

        [Test()]
        public void TestDeals1DamageToUpTo3Targets()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var bladeBattalion = PlayCard("BladeBattalion");
            var smg = PlayCard("SubmachineGun");

            DecisionSelectTargets = new Card[] { baron.CharacterCard, bladeBattalion, null };

            QuickHPStorage(baron.CharacterCard, bladeBattalion);
            UsePower(smg);
            QuickHPCheck(-1, -1);
        }

        [Test()]
        public void TestMacheteEffectDestroysOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var machete = PlayCard("Machete");
            var smg = PlayCard("SubmachineGun");
            var ongoing = PlayCard("LivingForceField");

            // Use Machete power first to activate {machete} effects
            DecisionSelectTarget = baron.CharacterCard;
            UsePower(machete);

            // Now use SMG - {machete} is active, can destroy ongoing
            DecisionSelectTargets = new Card[] { baron.CharacterCard, null, null };
            DecisionSelectCard = ongoing;
            UsePower(smg);

            AssertInTrash(ongoing);
        }

        [Test()]
        public void TestSniperEffectPutsTargetOnDeck()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var sniper = PlayCard("SniperRifle");
            var smg = PlayCard("SubmachineGun");
            var bladeBattalion = PlayCard("BladeBattalion");

            // Use Sniper power first to activate {sniper} effects
            PutInHand("CoverFire"); // Need a card to discard for sniper
            DecisionSelectTarget = baron.CharacterCard;
            UsePower(sniper);

            // Now use SMG - {sniper} is active, can put non-hero non-character target on deck
            DecisionSelectTargets = new Card[] { baron.CharacterCard, null, null };
            DecisionSelectCard = bladeBattalion;
            UsePower(smg);

            AssertOnTopOfDeck(bladeBattalion);
        }

        [Test()]
        public void TestNoEffectsWithoutActivation()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var smg = PlayCard("SubmachineGun");
            var ongoing = PlayCard("LivingForceField");
            var bladeBattalion = PlayCard("BladeBattalion");

            DecisionSelectTargets = new Card[] { baron.CharacterCard, null, null };

            UsePower(smg);

            // Without machete/sniper active, ongoing and battalion should still be in play
            AssertIsInPlay(ongoing);
            AssertIsInPlay(bladeBattalion);
        }
    }
}
