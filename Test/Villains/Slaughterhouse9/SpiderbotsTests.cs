using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Slaughterhouse9
{
    [TestFixture()]
    public class SpiderbotsTests : Slaughterhouse9TestBase
    {
        [Test()]
        public void TestEndOfTurnDamageScalesWithBots()
        {
            SetupAndStartNineGame();

            PlayCard("Spiderbots", 0);

            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 10);
            SetHitPoints(haka, 25);

            QuickHPStorage(legacy, bunker, haka);

            GoToEndOfTurn();

            // X = 1 + 1 Spiderbots in play.
            QuickHPCheck(0, -2, 0);
        }

        [Test()]
        public void TestTwoBotsHitHarder()
        {
            SetupAndStartNineGame();

            PlayCard("Spiderbots", 0);
            PlayCard("Spiderbots", 1);

            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 10);
            SetHitPoints(haka, 25);

            QuickHPStorage(legacy, bunker, haka);

            GoToEndOfTurn();

            // Each bot deals 1 + 2 = 3 to the lowest hero.
            QuickHPCheck(0, -6, 0);
        }
    }
}
