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
    public class AResponsePreparedTests : SimurghTestBase
    {
        [Test()]
        public void TestDestroysAllHeroOngoings()
        {
            SetupSimurghGame();
            RemoveSimurghTriggers();

            var sense = PlayCard("DangerSense");
            var presence = PlayCard("InspiringPresence");

            PlayCard("AResponsePrepared");

            AssertInTrash(sense);
            AssertInTrash(presence);
        }

        [Test()]
        public void TestPlaysWhenRevealed()
        {
            SetupSimurghGame();
            RemoveCountermeasures();

            var sense = PlayCard("DangerSense");

            // The Simurgh's start-of-turn reveal (H - 1 cards) will reveal it.
            var response = StackDeck("AResponsePrepared");

            GoToEndOfTurn();
            GoToStartOfTurn(simurgh);

            // It played itself out of the reveal, destroying the ongoing.
            AssertInTrash(response);
            AssertInTrash(sense);
        }
    }
}
