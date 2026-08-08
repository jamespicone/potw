using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.TheSimurgh
{
    [TestFixture()]
    public class ATerribleDefeatTests : SimurghTestBase
    {
        [Test()]
        public void TestFlipDestroysBusiestHerosCards()
        {
            SetupSimurghGame();
            RemoveSimurghTriggers();

            // Legacy has the most cards in play.
            var sense = PlayCard("DangerSense");
            var presence = PlayCard("InspiringPresence");
            var dominion = PlayCard("Dominion");

            FlipTrapWithPlanEnacted("ATerribleDefeat");

            AssertInTrash(sense);
            AssertInTrash(presence);
            AssertIsInPlay(dominion);
        }
    }
}
