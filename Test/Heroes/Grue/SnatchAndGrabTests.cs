using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Grue
{
    [TestFixture()]
    public class SnatchAndGrabTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "InsulaPrimalis");

            StartGame();
        }

        [Test()]
        public void TestDestroyUpTo2Ongoings()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var ongoing1 = PlayCard("Leadership");
            var ongoing2 = PlayCard("LivingForceField");
            AssertIsInPlay(ongoing1);
            AssertIsInPlay(ongoing2);

            DecisionSelectCards = new Card[] { ongoing1, ongoing2 };

            PlayCard("SnatchAndGrab");

            AssertInTrash(ongoing1);
            AssertInTrash(ongoing2);
        }

        [Test()]
        public void TestCanChoose0Ongoings()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var ongoing1 = PlayCard("Leadership");
            AssertIsInPlay(ongoing1);

            DecisionSelectCards = new Card[] { null, null };

            PlayCard("SnatchAndGrab");

            // Ongoing should still be in play (chose not to destroy)
            AssertIsInPlay(ongoing1);
        }

        [Test()]
        public void TestCanChoose1Ongoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var ongoing1 = PlayCard("Leadership");
            var ongoing2 = PlayCard("LivingForceField");
            AssertIsInPlay(ongoing1);
            AssertIsInPlay(ongoing2);

            DecisionSelectCards = new Card[] { ongoing1, null };

            PlayCard("SnatchAndGrab");

            AssertInTrash(ongoing1);
            AssertIsInPlay(ongoing2);
        }

        [Test()]
        public void TestCanTargetAnyOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            // Hero ongoing
            var heroOngoing = PlayCard("Leadership");
            // Villain ongoing
            var villainOngoing = PlayCard("LivingForceField");

            AssertIsInPlay(heroOngoing);
            AssertIsInPlay(villainOngoing);

            DecisionSelectCards = new Card[] { heroOngoing, villainOngoing };

            PlayCard("SnatchAndGrab");

            AssertInTrash(heroOngoing);
            AssertInTrash(villainOngoing);
        }

        [Test()]
        public void TestIsOneShot()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var card = GetCard("SnatchAndGrab");

            Assert.That(card.DoKeywordsContain("one-shot"), Is.True, "Snatch and Grab should be a one-shot");
        }
    }
}
