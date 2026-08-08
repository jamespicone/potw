using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Slaughterhouse9
{
    [TestFixture()]
    public class MurderRatTests : Slaughterhouse9TestBase
    {
        [Test()]
        public void TestEndOfTurnDamagesLowestHero()
        {
            SetupAndStartNineGame();

            var murderRat = PlayCard("MurderRat");

            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 10);
            SetHitPoints(haka, 25);

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageType(DamageType.Melee);
            AssertDamageSource(murderRat);

            GoToEndOfTurn();

            QuickHPCheck(0, -2, 0);
        }
    }
}
