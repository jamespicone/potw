using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Environment.CoilsBase
{
    [TestFixture()]
    public class SealedChamberTests : CoilsBaseTestBase
    {
        [Test()]
        public void TestDestructionEndsTheGame()
        {
            SetupCoilsBaseGame();

            var chamber = PlayCard("SealedChamber");

            DealDamage(haka, chamber, 25, DamageType.Melee);

            AssertGameOver(EndingResult.EnvironmentDefeat);
        }

        [Test()]
        public void TestPlayerMaySkipTurnToHeal()
        {
            SetupCoilsBaseGame();

            var chamber = PlayCard("SealedChamber");
            SetHitPoints(chamber, 10);

            DecisionsYesNo = new bool[] { true };
            GoToStartOfTurn(legacy);

            AssertHitPoints(chamber, 15);
        }

        [Test()]
        public void TestDecliningDoesNotHeal()
        {
            SetupCoilsBaseGame();

            var chamber = PlayCard("SealedChamber");
            SetHitPoints(chamber, 10);

            DecisionYesNo = false;
            GoToStartOfTurn(legacy);

            AssertHitPoints(chamber, 10);
        }
    }
}
