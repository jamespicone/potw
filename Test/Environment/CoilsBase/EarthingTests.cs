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
    public class EarthingTests : CoilsBaseTestBase
    {
        [Test()]
        public void TestReducesColdAndShocksOtherStructures()
        {
            SetupCoilsBaseGame();

            var earthing = PlayCard("Earthing");
            var cladding = PlayCard("AblativeCladding");

            QuickHPStorage(bunker.CharacterCard, earthing, cladding);

            DealDamage(haka, bunker, 4, DamageType.Cold);

            // Cold reduced by 2; every OTHER structure takes 1 irreducible energy.
            QuickHPCheck(-2, 0, -1);
        }

        [Test()]
        public void TestReducesLightning()
        {
            SetupCoilsBaseGame();

            var earthing = PlayCard("Earthing");

            QuickHPStorage(bunker.CharacterCard, earthing);

            DealDamage(haka, bunker, 4, DamageType.Lightning);

            QuickHPCheck(-2, 0);
        }

        [Test()]
        public void TestDoesNotReduceOtherTypes()
        {
            SetupCoilsBaseGame();

            PlayCard("Earthing");

            QuickHPStorage(bunker);

            DealDamage(haka, bunker, 4, DamageType.Fire);

            QuickHPCheck(-4);
        }
    }
}
