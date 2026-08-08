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
    public class TheOutskirtsTests : BrocktonBayTestBase
    {
        [Test()]
        public void TestEndOfTurnPlaysTopEnvironmentCard()
        {
            SetupBrocktonBayGame();

            var outskirts = PlayCard("TheOutskirts");
            var squad = StackDeck("PRTSquad");

            GoToEndOfTurn(env);

            AssertIsInPlay(squad);
            AssertIsInPlay(outskirts);
        }
    }
}
