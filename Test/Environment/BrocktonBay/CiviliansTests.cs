using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Environment.BrocktonBay
{
    [TestFixture()]
    public class CiviliansTests : BrocktonBayTestBase
    {
        [Test()]
        public void TestPlayersCannotUsePowers()
        {
            SetupBrocktonBayGame();

            PlayCard("Civilians");

            AssertNumberOfUsablePowers(legacy, 0);
            AssertNumberOfUsablePowers(bunker, 0);
            AssertNumberOfUsablePowers(haka, 0);
        }

        [Test()]
        public void TestEachPlayerMayDiscardToDestroy()
        {
            SetupBrocktonBayGame();

            var civilians = PlayCard("Civilians");

            QuickHandStorage(legacy, bunker, haka);
            DecisionYesNo = true;

            GoToStartOfTurn(env);

            QuickHandCheck(-1, -1, -1);
            AssertInTrash(civilians);
        }

        [Test()]
        public void TestDecliningKeepsCiviliansInPlay()
        {
            SetupBrocktonBayGame();

            var civilians = PlayCard("Civilians");

            QuickHandStorage(legacy, bunker, haka);
            DecisionYesNo = false;

            GoToStartOfTurn(env);

            QuickHandCheck(0, 0, 0);
            AssertIsInPlay(civilians);
        }
    }
}
