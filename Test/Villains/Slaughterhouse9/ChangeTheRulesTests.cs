using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Slaughterhouse9
{
    [TestFixture()]
    public class ChangeTheRulesTests : Slaughterhouse9TestBase
    {
        [Test()]
        public void TestRotatesNineMembers()
        {
            SetupAndStartNineGame();

            // H = 3 members deployed at setup, 5 under the Nine card.
            Assert.That(MembersInPlay.Count(), Is.EqualTo(3));

            // Make one member the unambiguous lowest-HP Nine target.
            var weakest = MembersInPlay.First();
            SetHitPoints(weakest, 3);

            PlayCard("ChangeTheRules", 0);

            // The weakest went back under; 2 came out: 3 - 1 + 2 = 4 in play,
            // and 4 remain under the Nine card.
            Assert.That(MembersInPlay.Count(), Is.EqualTo(4));
            Assert.That(nineCharacter.UnderLocation.NumberOfCards, Is.EqualTo(4));
        }
    }
}
