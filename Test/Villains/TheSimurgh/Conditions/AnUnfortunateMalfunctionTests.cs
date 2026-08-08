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
    public class AnUnfortunateMalfunctionTests : SimurghTestBase
    {
        [Test()]
        public void TestFlipsTrapWithEnoughEquipment()
        {
            SetupSimurghGame();
            RemoveSimurghTriggers();
            RemoveCountermeasures();

            PlayCard("FlakCannon");
            PlayCard("GatlingGun");
            PlayCard("HeavyPlating");

            var trap = PutTrapFaceDownInPlay("ADefencePenetrated");
            DecisionSelectCard = trap;

            var condition = PlayCard("AnUnfortunateMalfunction");

            Assert.That(trap.IsFlipped, Is.False, "The trap should have been flipped face up");
            AssertOutOfGame(condition);
        }

        [Test()]
        public void TestDoesNothingWithoutEnoughEquipment()
        {
            SetupSimurghGame();
            RemoveSimurghTriggers();
            RemoveCountermeasures();

            PlayCard("FlakCannon");

            var trap = PutTrapFaceDownInPlay("ADefencePenetrated");

            PlayCard("AnUnfortunateMalfunction");

            Assert.That(trap.IsFlipped, Is.True, "No trap should have been flipped");
        }
    }
}
