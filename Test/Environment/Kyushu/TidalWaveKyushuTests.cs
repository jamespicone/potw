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
    public class TidalWaveKyushuTests : KyushuTestBase
    {
        [Test()]
        public void TestDamagesHeroesAndForcesDiscard()
        {
            SetupKyushuGame();

            QuickHPStorage(legacy, bunker, haka);
            QuickHandStorage(legacy, bunker, haka);

            PlayCard("TidalWaveKyushu", 0);

            // 2 melee to each hero, then each damaged hero's player discards 1.
            QuickHPCheck(-2, -2, -2);
            QuickHandCheck(-1, -1, -1);
        }

        [Test()]
        public void TestSelfDestructsAtEndOfTurn()
        {
            SetupKyushuGame();

            var card = PlayCard("TidalWaveKyushu", 0);

            GoToEndOfTurn(env);

            AssertInTrash(card);
        }
    }
}
