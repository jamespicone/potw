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
    public class ConstantSmileTests : ParahumanTest
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

            var card = GetCard("ConstantSmile");
            Assert.That(card.DoKeywordsContain("ongoing"), Is.True);
            Assert.That(card.DoKeywordsContain("limited"), Is.True);
        }

        [Test()]
        public void TestStartOfTurnDeals1PsychicToNonHeroes()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();
            PlayCard("ConstantSmile");

            QuickHPStorage(baron);
            GoToStartOfTurn(tattletale);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestDoesNotDamageHeroes()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            PlayCard("ConstantSmile");

            QuickHPStorage(bunker);
            GoToStartOfTurn(tattletale);
            QuickHPCheck(0);
        }
    }
}
