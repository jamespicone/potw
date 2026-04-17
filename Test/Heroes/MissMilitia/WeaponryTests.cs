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
    public class WeaponryTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "InsulaPrimalis");
        }

        [Test()]
        public void TestIsOneShot()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "InsulaPrimalis");
            StartGame();

            var card = GetCard("Weaponry");
            Assert.That(card.IsOneShot, Is.True);
        }

        [Test()]
        public void TestSmgEffectIncreasesDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var smg = PlayCard("SubmachineGun");

            // Use SMG power to activate {smg} effects
            DecisionSelectTarget = baron.CharacterCard;
            UsePower(smg);

            // Play Weaponry - {smg} is active, increases MM damage by 1 this turn
            PlayCard("Weaponry");

            QuickHPStorage(baron);
            DealDamage(missmilitia, baron, 2, DamageType.Projectile);
            QuickHPCheck(-3); // 2 + 1 increase = 3
        }

        [Test()]
        public void TestMacheteEffectDeals1IrreducibleMelee()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var machete = PlayCard("Machete");

            // Use Machete power to activate {machete} effects
            DecisionSelectTarget = baron.CharacterCard;
            UsePower(machete);

            // Play Weaponry - {machete} is active, deals 1 irreducible melee
            QuickHPStorage(baron);
            PlayCard("Weaponry");
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestPistolEffectDrawsCard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var pistol = PlayCard("Pistol");

            // Use Pistol power to activate {pistol} effects
            DecisionSelectTarget = baron.CharacterCard;
            UsePower(pistol);

            // Play Weaponry - {pistol} is active, draws a card
            QuickHandStorage(missmilitia);
            PlayCard("Weaponry");
            QuickHandCheck(1); // +1 from draw (PlayCard plays from deck, not hand)
        }

        [Test()]
        public void TestSniperEffectDestroysTarget()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var sniper = PlayCard("SniperRifle");
            var bladeBattalion = PlayCard("BladeBattalion");

            // Use Sniper power to activate {sniper} effects
            PutInHand("CoverFire"); // Need a card to discard for sniper
            DecisionSelectTarget = baron.CharacterCard;
            UsePower(sniper);

            // Play Weaponry - {sniper} is active, can destroy non-character non-hero target
            DecisionSelectCard = bladeBattalion;
            PlayCard("Weaponry");

            AssertInTrash(bladeBattalion);
        }

        [Test()]
        public void TestNoEffectsWithoutWeaponActivation()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var bladeBattalion = PlayCard("BladeBattalion");

            // Play Weaponry without any weapon effects active - nothing should happen
            QuickHPStorage(baron.CharacterCard, bladeBattalion);
            QuickHandStorage(missmilitia);
            PlayCard("Weaponry");
            QuickHPCheck(0, 0);
            QuickHandCheck(0); // No effects, PlayCard plays from deck not hand
            AssertIsInPlay(bladeBattalion);
        }
    }
}
