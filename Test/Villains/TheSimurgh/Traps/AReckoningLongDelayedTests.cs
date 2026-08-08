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
    public class AReckoningLongDelayedTests : SimurghTestBase
    {
        [Test()]
        public void TestFlipMakesLowestHeroHitHighest()
        {
            SetupSimurghGame();
            RemoveSimurghTriggers();

            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 10);
            SetHitPoints(haka, 25);

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageSource(bunker.CharacterCard);
            AssertDamageType(DamageType.Projectile);
            AssertIrreducible();

            FlipTrapWithPlanEnacted("AReckoningLongDelayed");

            QuickHPCheck(0, 0, -5);
        }
    }
}
