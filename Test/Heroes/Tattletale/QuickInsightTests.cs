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
    public class QuickInsightTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();
        }

        [Test()]
        public void TestIsOngoingLimited()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            var card = GetCard("QuickInsight");
            Assert.That(card.DoKeywordsContain("ongoing"), Is.True);
            Assert.That(card.DoKeywordsContain("limited"), Is.True);
        }

        [Test()]
        public void TestTriggerOnTargetEnteringPlay()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            PlayCard("QuickInsight");

            // Select baron's deck to reveal from, put on top
            DecisionSelectLocation = new LocationChoice(baron.TurnTaker.Deck);
            DecisionMoveCardDestination = new MoveCardDestination(baron.TurnTaker.Deck);

            QuickHPStorage(tattletale);
            // Play a target to trigger Quick Insight
            PlayCard("BladeBattalion");
            // Should deal 1 psychic to self
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestCanDeclineReveal()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            PlayCard("QuickInsight");

            // Decline to select a deck (location selection, not card selection)
            DecisionDoNotSelectLocation = true;

            QuickHPStorage(tattletale);
            // Play a target to trigger Quick Insight
            PlayCard("BladeBattalion");
            // Should not take damage since we declined
            QuickHPCheck(0);
        }

        [Test()]
        public void TestDoesNotTriggerOnNonTarget()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            PlayCard("QuickInsight");

            QuickHPStorage(tattletale);
            // Play a non-target card (ongoing)
            PlayCard("Confidence");
            // Should not trigger
            QuickHPCheck(0);
        }
    }
}
