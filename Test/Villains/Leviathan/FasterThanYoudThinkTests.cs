using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Leviathan
{
    [TestFixture()]
    public class FasterThanYoudThinkTests : LeviathanTestBase
    {
        private Card SetupFasterThanYoudThink()
        {
            RemoveVillainTriggers();
            return PutTacticInPlay("FasterThanYoudThink");
        }

        [Test()]
        public void TestPreventsFirstDamageEachRound()
        {
            SetupLeviathanGame();
            SetupFasterThanYoudThink();

            QuickHPStorage(leviathan);
            DealDamage(haka, leviathan, 5, DamageType.Melee);
            QuickHPCheck(0);

            // Second damage in the same round goes through.
            DealDamage(haka, leviathan, 5, DamageType.Melee);
            QuickHPCheck(-5);
        }

        [Test()]
        public void TestPreventionResetsEachRound()
        {
            SetupLeviathanGame();
            RemoveEnvironmentDeck();
            SetupFasterThanYoudThink();

            QuickHPStorage(leviathan);
            DealDamage(haka, leviathan, 5, DamageType.Melee);
            QuickHPCheck(0);

            GoToEndOfTurn();
            GoToStartOfTurn(leviathan);

            QuickHPStorage(leviathan);
            DealDamage(haka, leviathan, 5, DamageType.Melee);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestIndestructible()
        {
            SetupLeviathanGame();
            var faster = SetupFasterThanYoudThink();

            DestroyCard(faster);

            AssertIsInPlay(faster);
        }
    }
}
