using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Legend
{
    [TestFixture()]
    public class RelativisticTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
        }

        [Test()]
        public void TestIsOneShot()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
            StartGame();

            var card = GetCard("Relativistic");
            Assert.That(card.IsOneShot, Is.True);
        }

        [Test()]
        public void TestPlaysUpTo2Cards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
            StartGame();

            var card1 = PutInHand("SkyHigh");
            var card2 = PutInHand("EnergyConversion");

            DecisionSelectCards = new Card[] { card1, card2 };

            PlayCard("Relativistic");

            AssertIsInPlay(card1);
            AssertIsInPlay(card2);
        }

        [Test()]
        public void TestCanDeclineToPlay()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
            StartGame();

            PutInHand("SkyHigh");
            // Ensure Relativistic is played from deck, not from hand
            PutOnDeck("Relativistic");

            DecisionDoNotSelectCard = SelectionType.PlayCard;

            var handCount = legend.HeroTurnTaker.Hand.Cards.Count();
            PlayCard("Relativistic");
            // Hand should not change (no cards played, Relativistic came from deck)
            Assert.That(legend.HeroTurnTaker.Hand.Cards.Count(), Is.EqualTo(handCount));
        }
    }
}
