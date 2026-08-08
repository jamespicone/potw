using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Environment.CoilsBase
{
    [TestFixture()]
    public class AblativeCladdingTests : CoilsBaseTestBase
    {
        [Test()]
        public void TestReducesMeleeAndAblates()
        {
            SetupCoilsBaseGame();

            var cladding = PlayCard("AblativeCladding");

            QuickHPStorage(bunker.CharacterCard, cladding);

            DealDamage(haka, bunker, 4, DamageType.Melee);

            // Damage reduced by 2, and the cladding chips itself for 1.
            QuickHPCheck(-2, -1);
        }

        [Test()]
        public void TestReducesToxic()
        {
            SetupCoilsBaseGame();

            var cladding = PlayCard("AblativeCladding");

            QuickHPStorage(bunker.CharacterCard, cladding);

            DealDamage(haka, bunker, 4, DamageType.Toxic);

            QuickHPCheck(-2, -1);
        }

        [Test()]
        public void TestDoesNotReduceOtherTypes()
        {
            SetupCoilsBaseGame();

            var cladding = PlayCard("AblativeCladding");

            QuickHPStorage(bunker.CharacterCard, cladding);

            DealDamage(haka, bunker, 4, DamageType.Fire);

            QuickHPCheck(-4, 0);
        }
    }
}
