using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Lung
{
    [TestFixture()]
    public class ABBThugsTests : LungTestBase
    {
        [Test()]
        public void TestEndOfTurnDamagesLowestHero()
        {
            SetupLungGame();
            RemoveLungTriggers();

            var thugs = PlayCard("ABBThugs");

            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 15);
            SetHitPoints(haka, 25);

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageSource(thugs);
            AssertDamageType(DamageType.Projectile);

            GoToEndOfTurn();

            QuickHPCheck(0, -2, 0);
        }
    }
}
