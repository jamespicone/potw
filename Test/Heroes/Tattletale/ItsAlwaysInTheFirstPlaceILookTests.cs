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
    public class ItsAlwaysInTheFirstPlaceILookTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();
        }

        [Test()]
        public void TestIsOneShot()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            var card = GetCard("ItsAlwaysInTheFirstPlaceILook");
            Assert.That(card.DoKeywordsContain("one-shot"), Is.True);
        }

        [Test()]
        public void TestSearchDeckForCard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            var confidence = PutOnDeck("Confidence");

            DecisionSelectTurnTaker = tattletale.TurnTaker;
            DecisionSelectCard = confidence;

            PlayCard("ItsAlwaysInTheFirstPlaceILook");
            AssertInHand(confidence);
        }

        [Test()]
        public void TestCanTargetOtherPlayer()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            var bunkerCard = bunker.TurnTaker.Deck.TopCard;

            DecisionSelectTurnTaker = bunker.TurnTaker;
            DecisionSelectCard = bunkerCard;

            PlayCard("ItsAlwaysInTheFirstPlaceILook");
            AssertInHand(bunkerCard);
        }
    }
}
