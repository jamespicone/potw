using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Leviathan
{
    [TestFixture()]
    public class CunningTests : LeviathanTestBase
    {
        [Test()]
        public void TestPlaysTopTwoCards()
        {
            SetupLeviathanGame();
            RemoveVillainTriggers();
            MoveTacticsToDeckBottom();

            var stacked = StackDeckHandleDuplicates("ImpossibleStrength", "ImpossibleToughness").ToList();

            PlayCard("Cunning");

            AssertIsInPlay(stacked[0]);
            AssertIsInPlay(stacked[1]);
        }
    }
}
