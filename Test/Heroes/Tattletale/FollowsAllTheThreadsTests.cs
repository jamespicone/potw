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
    public class FollowsAllTheThreadsTests : ParahumanTest
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

            var card = GetCard("FollowsAllTheThreads");
            Assert.That(card.DoKeywordsContain("ongoing"), Is.True);
            Assert.That(card.DoKeywordsContain("limited"), Is.True);
        }

        [Test()]
        public void TestTopCardsFaceUp()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            PlayCard("FollowsAllTheThreads");

            Assert.That(baron.TurnTaker.Deck.TopCard.IsFaceUp, Is.True);
            Assert.That(bunker.TurnTaker.Deck.TopCard.IsFaceUp, Is.True);
        }

        [Test()]
        public void TestStartOfTurnSelfDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            PlayCard("FollowsAllTheThreads");

            QuickHPStorage(tattletale);
            GoToStartOfTurn(tattletale);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestPowerDestroysSelf()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            var card = PlayCard("FollowsAllTheThreads");
            AssertIsInPlay(card);

            UsePower(card);
            AssertInTrash(card);
        }
    }
}
