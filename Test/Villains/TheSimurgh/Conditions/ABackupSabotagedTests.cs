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
    public class ABackupSabotagedTests : SimurghTestBase
    {
        [Test()]
        public void TestFlipsTrapWhenHeroHasDuplicateCardsInHand()
        {
            SetupSimurghGame();
            RemoveSimurghTriggers();
            RemoveCountermeasures();

            // Two copies of the same card in Legacy's hand.
            PutInHand("Fortitude");
            PutInHand("Fortitude");

            var trap = PutTrapFaceDownInPlay("ADefencePenetrated");
            DecisionSelectCard = trap;

            var condition = PlayCard("ABackupSabotaged");

            Assert.That(trap.IsFlipped, Is.False, "The trap should have been flipped face up");
            AssertOutOfGame(condition);
        }
    }
}
