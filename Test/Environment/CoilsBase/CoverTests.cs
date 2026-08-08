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
    public class CoverTests : CoilsBaseTestBase
    {
        [Test()]
        public void TestRedirectsNonEnvironmentDamage()
        {
            SetupCoilsBaseGame();

            var cover = PlayCard("Cover");

            QuickHPStorage(bunker.CharacterCard, cover);

            DealDamage(haka, bunker, 3, DamageType.Melee);

            // Redirected to Cover, which also reduces damage to itself by 1.
            QuickHPCheck(0, -2);
        }

        [Test()]
        public void TestDoesNotRedirectEnvironmentDamage()
        {
            SetupCoilsBaseGame();

            var cover = PlayCard("Cover");
            var mercs = PlayCard("Mercenaries");

            QuickHPStorage(bunker.CharacterCard, cover);

            DealDamage(mercs, bunker.CharacterCard, 3, DamageType.Energy);

            QuickHPCheck(-3, 0);
        }
    }
}
