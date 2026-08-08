using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Environment.NewDelhi
{
    [TestFixture()]
    public class UnstoppableTests : NewDelhiTestBase
    {
        [Test()]
        public void TestReducesDamageToHighestVillain()
        {
            SetupNewDelhiGame();

            var unstoppable = PlayCard("Unstoppable");

            QuickHPStorage(baron);
            DealDamage(haka, baron, 3, DamageType.Melee);
            QuickHPCheck(-2);

            AssertIsInPlay(unstoppable);
        }

        [Test()]
        public void TestDestroyedByBigHit()
        {
            SetupNewDelhiGame();

            var unstoppable = PlayCard("Unstoppable");

            QuickHPStorage(baron);
            DealDamage(haka, baron, 6, DamageType.Melee);

            // 6 reduced to 5, which is 4+ damage at once: Unstoppable is destroyed.
            QuickHPCheck(-5);
            AssertInTrash(unstoppable);
        }
    }
}
