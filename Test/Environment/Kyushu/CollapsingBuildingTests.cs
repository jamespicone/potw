using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Environment.Kyushu
{
    [TestFixture()]
    public class CollapsingBuildingTests : KyushuTestBase
    {
        [Test()]
        public void TestEndOfTurnDamagesSecondHighestAndSelfDestructs()
        {
            SetupKyushuGame();

            // Decline the on-enter discard-to-destroy offer.
            DecisionDoNotSelectTurnTaker = true;
            var building = PlayCard("CollapsingBuilding", 0);

            // Baron Blade (40) is the highest target; Haka is second.
            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 10);
            SetHitPoints(haka, 30);

            QuickHPStorage(baron.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, haka.CharacterCard);

            GoToEndOfTurn(env);

            // 4 irreducible melee to the second-highest target, then self-destruct.
            QuickHPCheck(0, 0, 0, -4);
            AssertInTrash(building);
        }

        [Test()]
        public void TestPlayerMayDiscardTwoToDestroyOnEnter()
        {
            SetupKyushuGame();

            DecisionSelectTurnTaker = legacy.TurnTaker;
            QuickHandStorage(legacy);

            var building = PlayCard("CollapsingBuilding", 0);

            QuickHandCheck(-2);
            AssertInTrash(building);
        }
    }
}
