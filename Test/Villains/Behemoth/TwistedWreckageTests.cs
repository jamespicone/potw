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
    public class TwistedWreckageTests : BehemothTestBase
    {
        [Test()]
        public void TestDestroysRemovesAndMovesToken()
        {
            SetupBehemothGame();
            RemoveBehemothTriggers();
            ClearProximity();
            SetProximity(bunker, 1);

            // The only two hero noncharacter cards in play, both Legacy's.
            var sense = PlayCard("DangerSense");
            var presence = PlayCard("InspiringPresence");

            // First destroy pick is Danger Sense (the second is forced), then remove
            // Danger Sense from the game.
            DecisionSelectCards = new Card[] { sense, sense };

            PlayCard("TwistedWreckage");

            AssertOutOfGame(sense);
            AssertInTrash(presence);

            // A token moved from Bunker (the only other hero with one) to Legacy.
            Assert.That(Proximity(legacy).CurrentValue, Is.EqualTo(1));
            Assert.That(Proximity(bunker).CurrentValue, Is.EqualTo(0));
        }
    }
}
