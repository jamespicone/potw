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
    public class SomeMoreFriendsTests : Slaughterhouse9TestBase
    {
        [Test()]
        public void TestDeploysMemberAndPlaysTopCard()
        {
            SetupAndStartNineGame();

            int before = MembersInPlay.Count();
            var legendary = StackDeck("Legendary");

            PlayCard("SomeMoreFriends", 0);

            Assert.That(MembersInPlay.Count(), Is.EqualTo(before + 1));
            AssertIsInPlay(legendary);
        }
    }
}
