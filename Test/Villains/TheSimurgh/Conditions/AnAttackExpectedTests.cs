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
    public class AnAttackExpectedTests : SimurghTestBase
    {
        [Test()]
        public void TestFlipsTrapWhenSimurghWasDamaged()
        {
            SetupSimurghGame();
            RemoveSimurghTriggers();
            RemoveCountermeasures();

            // "Was dealt damage last round" counts damage since the villain's last
            // End phase, so deal it during a hero turn and come back around.
            GoToStartOfTurn(legacy);
            DealDamage(haka, simurgh, 3, DamageType.Melee);
            GoToStartOfTurn(simurgh);

            var trap = PutTrapFaceDownInPlay("ADefencePenetrated");
            DecisionSelectCard = trap;

            var condition = PlayCard("AnAttackExpected");

            Assert.That(trap.IsFlipped, Is.False, "The trap should have been flipped face up");
            AssertOutOfGame(condition);
        }

        [Test()]
        public void TestDoesNothingWithoutDamage()
        {
            SetupSimurghGame();
            RemoveSimurghTriggers();
            RemoveCountermeasures();

            var trap = PutTrapFaceDownInPlay("ADefencePenetrated");

            PlayCard("AnAttackExpected");

            Assert.That(trap.IsFlipped, Is.True, "No trap should have been flipped");
        }
    }
}
