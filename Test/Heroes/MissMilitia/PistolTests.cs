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
    public class PistolTests : ParahumanTest
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

            var card = GetCard("Pistol");
            Assert.That(card.DoKeywordsContain("weapon"), Is.True);
            Assert.That(card.DoKeywordsContain("equipment"), Is.True);
        }

        [Test()]
        public void TestDeals2ProjectileDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var pistol = PlayCard("Pistol");

            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            UsePower(pistol);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestSmgEffectAllowsPlayCard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var smg = PlayCard("SubmachineGun");
            var pistol = PlayCard("Pistol");

            // Use SMG power first to activate {smg} effects
            DecisionSelectTargets = new Card[] { baron.CharacterCard, null, null, baron.CharacterCard };
            UsePower(smg);

            // Now use Pistol - {smg} is active, so we can play a card
            var cardToPlay = PutInHand("IDontSleep");
            DecisionSelectCard = cardToPlay;

            UsePower(pistol);

            AssertIsInPlay(cardToPlay);
        }

        [Test()]
        public void TestNoSmgEffectWithoutSmgActive()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var pistol = PlayCard("Pistol");
            var cardInHand = PutInHand("CoverFire");

            DecisionSelectTarget = baron.CharacterCard;

            UsePower(pistol);

            // Without SMG active, CoverFire should stay in hand
            AssertInHand(cardInHand);
        }
    }
}
