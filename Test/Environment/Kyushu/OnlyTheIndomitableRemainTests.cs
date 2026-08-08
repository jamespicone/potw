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
    public class OnlyTheIndomitableRemainTests : KyushuTestBase
    {
        [Test()]
        public void TestReducesAllDamage()
        {
            SetupKyushuGame();

            PlayCard("OnlyTheIndomitableRemain");

            QuickHPStorage(bunker);
            DealDamage(haka, bunker, 4, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestEndOfTurnHealsAllTargets()
        {
            SetupKyushuGame();

            PlayCard("OnlyTheIndomitableRemain");

            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 10);
            SetHitPoints(haka, 25);

            QuickHPStorage(legacy, bunker, haka);

            GoToEndOfTurn(env);

            QuickHPCheck(1, 1, 1);
        }

        [Test()]
        public void TestDestroyedWhenATargetIsDestroyed()
        {
            SetupKyushuGame();

            var indomitable = PlayCard("OnlyTheIndomitableRemain");
            var sentai = PlayCard("SentaiElite", 0);

            DestroyCard(sentai);

            AssertInTrash(indomitable);
        }
    }
}
