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
    public class SkyHighTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
        }

        [Test()]
        public void TestIsOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
            StartGame();

            var card = GetCard("SkyHigh");
            Assert.That(card.DoKeywordsContain("ongoing"), Is.True);
        }

        [Test()]
        public void TestLegendInvisibleToEnvironmentCards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
            StartGame();

            PlayCard("SkyHigh");

            var envCard = PlayCard("VelociraptorPack");

            var cardSource = new CardSource(FindCardController(envCard));
            Assert.That(GameController.IsCardVisibleToCardSource(legend.CharacterCard, cardSource), Is.False);
        }

        [Test()]
        public void TestLegendCardsInvisibleToEnvironmentCards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
            StartGame();

            var skyHigh = PlayCard("SkyHigh");
            var bounceBack = PlayCard("BounceBack");

            var envCard = PlayCard("VelociraptorPack");

            var cardSource = new CardSource(FindCardController(envCard));
            Assert.That(GameController.IsCardVisibleToCardSource(skyHigh, cardSource), Is.False);
            Assert.That(GameController.IsCardVisibleToCardSource(bounceBack, cardSource), Is.False);
        }

        [Test()]
        public void TestLegendVisibleToVillainCards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
            StartGame();

            PlayCard("SkyHigh");

            var cardSource = new CardSource(FindCardController(baron.CharacterCard));
            Assert.That(GameController.IsCardVisibleToCardSource(legend.CharacterCard, cardSource), Is.True);
        }

        [Test()]
        public void TestLegendVisibleToHeroCards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            PlayCard("SkyHigh");

            var cardSource = new CardSource(FindCardController(bunker.CharacterCard));
            Assert.That(GameController.IsCardVisibleToCardSource(legend.CharacterCard, cardSource), Is.True);
        }
    }
}
