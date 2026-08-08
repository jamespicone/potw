using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Environment.NewDelhi
{
    [TestFixture()]
    public class ThandaTests : NewDelhiTestBase
    {
        [Test()]
        public void TestRemovesLowestParahumanAndLosesToken()
        {
            SetupNewDelhiGame();

            var thanda = PlayCard("Thanda");
            var phirSe = PlayCard("PhirSe"); // 5 HP noncharacter parahuman

            GoToEndOfTurn(env);

            AssertOutOfGame(phirSe);
            Assert.That(thanda.FindTokenPool("ThandaPool").CurrentValue, Is.EqualTo(2));
        }

        [Test()]
        public void TestKeepsTokensWithNoParahumans()
        {
            SetupNewDelhiGame();

            var thanda = PlayCard("Thanda");

            GoToEndOfTurn(env);

            Assert.That(thanda.FindTokenPool("ThandaPool").CurrentValue, Is.EqualTo(3));
        }

        [Test()]
        public void TestRemovedFromGameAtZeroTokens()
        {
            SetupNewDelhiGame();

            var thanda = PlayCard("Thanda");
            thanda.FindTokenPool("ThandaPool").SetNumberOfTokens(1);
            PlayCard("PhirSe");

            GoToEndOfTurn(env);

            AssertOutOfGame(thanda);
        }
    }
}
