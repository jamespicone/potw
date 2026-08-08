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
    public class ACrucialHesitationTests : SimurghTestBase
    {
        [Test()]
        public void TestFlipSetsTokens()
        {
            SetupSimurghGame();
            RemoveSimurghTriggers();

            var trap = FlipTrapWithPlanEnacted("ACrucialHesitation");

            // 8 - H = 5 tokens.
            Assert.That(trap.FindTokenPool("ACrucialHesitationPool").CurrentValue, Is.EqualTo(5));
        }

        [Test()]
        public void TestEndOfVillainTurnRemovesToken()
        {
            SetupSimurghGame();
            RemoveSimurghTriggers();

            var trap = FlipTrapWithPlanEnacted("ACrucialHesitation");

            GoToEndOfTurn();

            Assert.That(trap.FindTokenPool("ACrucialHesitationPool").CurrentValue, Is.EqualTo(4));
        }

        [Test()]
        public void TestHeroesLoseAtZeroTokens()
        {
            SetupSimurghGame();
            RemoveSimurghTriggers();

            var trap = FlipTrapWithPlanEnacted("ACrucialHesitation");
            trap.FindTokenPool("ACrucialHesitationPool").SetNumberOfTokens(1);

            GoToEndOfTurn();

            AssertGameOver(EndingResult.AlternateDefeat);
        }
    }
}
