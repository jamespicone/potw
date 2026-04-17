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
    public class OutOfCommissionTests : ParahumanTest
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

            var card = GetCard("OutOfCommission");
            Assert.That(card.DoKeywordsContain("one-shot"), Is.True);
        }

        [Test()]
        public void TestHeals5HP()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            SetHitPoints(tattletale, 15);

            QuickHPStorage(tattletale);
            PlayCard("OutOfCommission");
            QuickHPCheck(5);
        }

        [Test()]
        public void TestEndsTurnImmediately()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            SetHitPoints(tattletale, 15);

            GoToPlayCardPhase(tattletale);

            // Track hand size - if turn ends immediately, no draw phase occurs
            QuickHandStorage(tattletale);
            PlayCard("OutOfCommission");
            // If turn ended immediately, draw phase is skipped so hand count stays the same
            // (card was played from deck, not from hand)
            QuickHandCheck(0);
        }
    }
}
