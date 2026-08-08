using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Behemoth
{
    [TestFixture()]
    public class IncinerateTests : BehemothTestBase
    {
        [Test()]
        public void TestDestroysOngoingsAndMovesToken()
        {
            SetupBehemothGame();
            RemoveBehemothTriggers();
            ClearProximity();
            SetProximity(legacy, 2);

            // The only two hero ongoings in play, both Legacy's.
            var sense = PlayCard("DangerSense");
            var presence = PlayCard("InspiringPresence");

            // Legacy is the only hero who lost cards and has tokens, so he passes;
            // he chooses to give the token to Bunker.
            DecisionSelectTurnTaker = bunker.TurnTaker;

            PlayCard("Incinerate");

            AssertInTrash(sense);
            AssertInTrash(presence);

            Assert.That(Proximity(legacy).CurrentValue, Is.EqualTo(1));
            Assert.That(Proximity(bunker).CurrentValue, Is.EqualTo(1));
        }
    }
}
