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
    public class SniperRifleTests : ParahumanTest
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

            var card = GetCard("SniperRifle");
            Assert.That(card.DoKeywordsContain("weapon"), Is.True);
            Assert.That(card.DoKeywordsContain("equipment"), Is.True);
        }

        [Test()]
        public void TestDiscardsAndDealsDelayedDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var sniper = PlayCard("SniperRifle");
            var cardToDiscard = PutInHand("CoverFire");

            DecisionSelectCard = cardToDiscard;
            DecisionSelectTarget = baron.CharacterCard;

            UsePower(sniper);

            // Card should be discarded
            AssertInTrash(cardToDiscard);

            // Damage happens at start of next turn
            QuickHPStorage(baron);
            GoToStartOfTurn(missmilitia);
            QuickHPCheck(-4);
        }

        [Test()]
        public void TestPistolEffectDraws3Cards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var pistol = PlayCard("Pistol");
            var sniper = PlayCard("SniperRifle");

            // Use Pistol power first to activate {pistol} effects
            DecisionSelectTarget = baron.CharacterCard;
            UsePower(pistol);

            // Now use Sniper - {pistol} is active, draws 3 cards
            var cardToDiscard = PutInHand("CoverFire");
            DecisionSelectCard = cardToDiscard;

            QuickHandStorage(missmilitia);
            UsePower(sniper);
            // -1 discard + 3 draw = +2 net
            QuickHandCheck(2);
        }

        [Test()]
        public void TestNoPistolEffectWithoutPistolActive()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var sniper = PlayCard("SniperRifle");
            var cardToDiscard = PutInHand("CoverFire");

            DecisionSelectCard = cardToDiscard;
            DecisionSelectTarget = baron.CharacterCard;

            QuickHandStorage(missmilitia);
            UsePower(sniper);
            // Only -1 discard, no draw
            QuickHandCheck(-1);
        }
    }
}
