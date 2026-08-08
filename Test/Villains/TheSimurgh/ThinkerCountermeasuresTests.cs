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
    public class ThinkerCountermeasuresTests : SimurghTestBase
    {
        [Test()]
        public void TestCancelsTrapFlipAndRemovesItself()
        {
            SetupSimurghGame();
            RemoveSimurghTriggers();

            // 5 - H = 2 countermeasures start in play, but the random condition
            // played at the start of villain turn 1 can occasionally consume one,
            // so count what's actually there.
            var before = FindCardsWhere(c => c.Identifier == "ThinkerCountermeasures" && c.IsInPlay).Count();
            Assert.That(before, Is.GreaterThan(0), "Expected at least one countermeasure in play");

            var trap = PutTrapFaceDownInPlay("ADefencePenetrated");
            DecisionSelectCard = trap;

            PlayCard("APlanEnacted");

            // The flip was cancelled at the cost of a countermeasure.
            Assert.That(trap.IsFlipped, Is.True, "The trap should still be face down");
            AssertNumberOfCardsInPlay("ThinkerCountermeasures", before - 1);
        }
    }
}
