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
    public class APlanEnactedTests : SimurghTestBase
    {
        [Test()]
        public void TestFlipsFaceDownCardAndShufflesIntoDeck()
        {
            SetupSimurghGame();
            RemoveSimurghTriggers();
            RemoveCountermeasures();

            var trap = PutTrapFaceDownInPlay("ADefencePenetrated");
            DecisionSelectCard = trap;

            var plan = PlayCard("APlanEnacted");

            Assert.That(trap.IsFlipped, Is.False, "The trap should have been flipped face up");
            AssertInDeck(plan);
        }

        [Test()]
        public void TestPlaysTopCardWhenNothingFaceDown()
        {
            SetupSimurghGame();
            RemoveSimurghTriggers();
            RemoveCountermeasures();

            foreach (var trap in FaceDownTrapsInPlay.ToList())
            {
                FlipCard(trap);
            }

            var fate = StackDeck("AFateSelected");

            PlayCard("APlanEnacted");

            // With no face-down villain cards, the top card of the villain deck
            // was played instead.
            AssertInTrash(fate);
        }
    }
}
