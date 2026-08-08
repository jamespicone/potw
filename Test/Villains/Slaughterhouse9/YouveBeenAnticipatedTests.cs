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
    public class YouveBeenAnticipatedTests : Slaughterhouse9TestBase
    {
        [Test()]
        public void TestEachPlayerDiscardsTwo()
        {
            SetupAndStartNineGame();

            QuickHandStorage(legacy, bunker, haka);

            PlayCard("YouveBeenAnticipated", 0);

            QuickHandCheck(-2, -2, -2);
        }
    }
}
