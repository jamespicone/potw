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
    public class TorrentialDownpourTests : LeviathanTestBase
    {
        private Card SetupTorrentialDownpour()
        {
            RemoveVillainTriggers();
            return PutTacticInPlay("TorrentialDownpour");
        }

        [Test()]
        public void TestDamageOnDraw()
        {
            SetupLeviathanGame();
            SetupTorrentialDownpour();

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageType(DamageType.Cold);
            AssertDamageSource(leviathan.CharacterCard);

            DrawCard(legacy);

            QuickHPCheck(-1, 0, 0);
        }

        [Test()]
        public void TestEachDrawTriggersDamage()
        {
            SetupLeviathanGame();
            SetupTorrentialDownpour();

            QuickHPStorage(legacy);
            DrawCard(legacy, 2);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestIndestructible()
        {
            SetupLeviathanGame();
            var downpour = SetupTorrentialDownpour();

            DestroyCard(downpour);

            AssertIsInPlay(downpour);
        }
    }
}
