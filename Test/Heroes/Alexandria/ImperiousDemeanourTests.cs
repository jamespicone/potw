using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Alexandria
{
    [TestFixture()]
    public class ImperiousDemeanourTests : ParahumanTest
    {
        [Test()]
        public void TestDraws()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "InsulaPrimalis");

            StartGame();

            RemoveVillainTriggers();
            RemoveVillainCards();

            MoveAllCardsFromHandToDeck(alexandria);
            QuickHandStorage(alexandria);
            PlayCard("ImperiousDemeanour");
            QuickHandCheck(1);
        }

        [Test()]
        public void TestHealsOnPowerUse()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "InsulaPrimalis");

            StartGame();

            RemoveVillainTriggers();
            RemoveVillainCards();

            PlayCard("ImperiousDemeanour");
            MoveAllCardsFromHandToDeck(alexandria);

            SetHitPoints(alexandria, 10);

            QuickHPStorage(alexandria);
            UsePower(alexandria);
            QuickHPCheck(3);
        }
    }
}
