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
    public class AnAmbushLaidTests : SimurghTestBase
    {
        [Test()]
        public void TestDamageScalesWithFaceUpTraps()
        {
            SetupSimurghGame();
            RemoveSimurghTriggers();

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageType(DamageType.Fire, DamageType.Fire, DamageType.Fire);

            // No face-up traps: X = 1.
            PlayCard("AnAmbushLaid", 0);
            QuickHPCheck(-1, -1, -1);

            FlipTrapFaceUpDormant("ADefencePenetrated");

            // One face-up trap: X = 2.
            PlayCard("AnAmbushLaid", 1);
            QuickHPCheck(-2, -2, -2);
        }
    }
}
