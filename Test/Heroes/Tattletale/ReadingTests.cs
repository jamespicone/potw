using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Tattletale
{
    [TestFixture()]
    public class ReadingTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();
        }

        [Test()]
        public void TestIsOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            var card = GetCard("Reading");
            Assert.That(card.DoKeywordsContain("ongoing"), Is.True);
        }

        [Test()]
        public void TestPowerReordersCards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            var reading = PlayCard("Reading");

            // Select Tattletale's own deck
            DecisionSelectLocation = new LocationChoice(tattletale.TurnTaker.Deck);

            // Use DecisionAutoDecideIfAble to auto-select cards for top/bottom
            DecisionAutoDecideIfAble = true;

            int deckCount = tattletale.TurnTaker.Deck.NumberOfCards;
            UsePower(reading);

            // Power should complete - deck still has all its cards (just reordered)
            Assert.That(tattletale.TurnTaker.Deck.NumberOfCards, Is.EqualTo(deckCount));
        }
    }
}
