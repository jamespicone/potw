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
    public class ASurfeitOfOptionsTests : SimurghTestBase
    {
        [Test()]
        public void TestFlipsTrapWithEnoughOngoings()
        {
            SetupSimurghGame();
            RemoveSimurghTriggers();
            RemoveCountermeasures();

            PlayCard("DangerSense");
            PlayCard("InspiringPresence");
            PlayCard("Dominion");

            var trap = PutTrapFaceDownInPlay("ADefencePenetrated");
            DecisionSelectCard = trap;

            var condition = PlayCard("ASurfeitOfOptions");

            Assert.That(trap.IsFlipped, Is.False, "The trap should have been flipped face up");
            AssertOutOfGame(condition);
        }

        [Test()]
        public void TestDoesNothingWithoutEnoughOngoings()
        {
            SetupSimurghGame();
            RemoveSimurghTriggers();
            RemoveCountermeasures();

            PlayCard("DangerSense");

            var trap = PutTrapFaceDownInPlay("ADefencePenetrated");

            PlayCard("ASurfeitOfOptions");

            Assert.That(trap.IsFlipped, Is.True, "No trap should have been flipped");
        }
    }
}
