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
    public class IDontSleepTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "InsulaPrimalis");
        }

        [Test()]
        public void TestIsOngoingLimited()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "InsulaPrimalis");
            StartGame();

            var card = GetCard("IDontSleep");
            Assert.That(card.DoKeywordsContain("ongoing"), Is.True);
            Assert.That(card.DoKeywordsContain("limited"), Is.True);
        }

        [Test()]
        public void TestDrawsCardAtEndOfTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            PlayCard("IDontSleep");

            QuickHandStorage(missmilitia);
            GoToEndOfTurn(missmilitia);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestDoesNotDrawOnOtherTurns()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            PlayCard("IDontSleep");

            GoToEndOfTurn(missmilitia);

            QuickHandStorage(missmilitia);
            GoToEndOfTurn(bunker);
            QuickHandCheck(0);
        }
    }
}
