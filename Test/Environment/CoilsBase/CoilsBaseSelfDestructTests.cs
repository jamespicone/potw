using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Environment.CoilsBase
{
    [TestFixture()]
    public class CoilsBaseSelfDestructTests : CoilsBaseTestBase
    {
        [Test()]
        public void TestPlayerMaySkipTurnToDestroy()
        {
            SetupCoilsBaseGame();

            var protocols = PlayCard("StrangerAndMasterProtocols");

            DecisionsYesNo = new bool[] { true };
            GoToStartOfTurn(legacy);

            AssertInTrash(protocols);
        }

        [Test()]
        public void TestDecliningKeepsCardInPlay()
        {
            SetupCoilsBaseGame();

            var protocols = PlayCard("StrangerAndMasterProtocols");

            DecisionYesNo = false;
            GoToStartOfTurn(legacy);

            AssertIsInPlay(protocols);
        }
    }
}
