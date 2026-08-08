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
    public class AWealthOfTargetsTests : SimurghTestBase
    {
        [Test()]
        public void TestFlipsTrapWithEnoughTargets()
        {
            SetupSimurghGame();
            RemoveSimurghTriggers();
            RemoveCountermeasures();

            PlayCard("TrafficPileup");
            PlayCard("PlummetingMonorail");
            PlayCard("TargetingInnocents");

            var trap = PutTrapFaceDownInPlay("ADefencePenetrated");
            DecisionSelectCard = trap;

            var condition = PlayCard("AWealthOfTargets");

            Assert.That(trap.IsFlipped, Is.False, "The trap should have been flipped face up");
            AssertOutOfGame(condition);
        }

        [Test()]
        public void TestDoesNothingWithoutEnoughTargets()
        {
            SetupSimurghGame();
            RemoveSimurghTriggers();
            RemoveCountermeasures();

            PlayCard("TrafficPileup");

            var trap = PutTrapFaceDownInPlay("ADefencePenetrated");

            PlayCard("AWealthOfTargets");

            Assert.That(trap.IsFlipped, Is.True, "No trap should have been flipped");
        }
    }
}
