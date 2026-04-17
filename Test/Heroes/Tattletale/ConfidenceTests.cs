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
    public class ConfidenceTests : ParahumanTest
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

            var card = GetCard("Confidence");
            Assert.That(card.DoKeywordsContain("ongoing"), Is.True);
            Assert.That(card.DoKeywordsContain("limited"), Is.True);
        }

        [Test()]
        public void TestStartOfTurnHeroesRegain1HP()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            SetHitPoints(tattletale, 20);
            SetHitPoints(bunker, 20);

            PlayCard("Confidence");

            QuickHPStorage(tattletale, bunker);
            GoToStartOfTurn(tattletale);
            QuickHPCheck(1, 1);
        }

        [Test()]
        public void TestDoesNotHealAtFullHP()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            PlayCard("Confidence");

            QuickHPStorage(tattletale, bunker);
            GoToStartOfTurn(tattletale);
            QuickHPCheck(0, 0);
        }
    }
}
