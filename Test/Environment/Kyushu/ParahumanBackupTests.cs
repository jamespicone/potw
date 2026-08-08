using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Environment.Kyushu
{
    [TestFixture()]
    public class ParahumanBackupTests : KyushuTestBase
    {
        [Test()]
        public void TestEachPlayerMayDraw()
        {
            SetupKyushuGame();

            QuickHandStorage(legacy, bunker, haka);
            DecisionYesNo = true;

            PlayCard("ParahumanBackup", 0);

            QuickHandCheck(1, 1, 1);
        }

        [Test()]
        public void TestSelfDestructsAtEndOfTurn()
        {
            SetupKyushuGame();

            DecisionYesNo = false;
            var card = PlayCard("ParahumanBackup", 0);

            GoToEndOfTurn(env);

            AssertInTrash(card);
        }
    }
}
