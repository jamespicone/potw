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
    public class AStrengthTurnedToWeaknessTests : SimurghTestBase
    {
        [Test()]
        public void TestBigHitCausesCounterDamage()
        {
            SetupSimurghGame();
            RemoveSimurghTriggers();

            FlipTrapFaceUpDormant("AStrengthTurnedToWeakness");

            QuickHPStorage(haka);

            // 9 damage dealt: more than 8 - H = 5, so Haka punches himself for 1.
            DealDamage(haka, simurgh, 9, DamageType.Melee);

            QuickHPCheck(-1);
        }

        [Test()]
        public void TestSmallHitCausesNoCounterDamage()
        {
            SetupSimurghGame();
            RemoveSimurghTriggers();

            FlipTrapFaceUpDormant("AStrengthTurnedToWeakness");

            QuickHPStorage(haka);

            DealDamage(haka, simurgh, 5, DamageType.Melee);

            QuickHPCheck(0);
        }
    }
}
