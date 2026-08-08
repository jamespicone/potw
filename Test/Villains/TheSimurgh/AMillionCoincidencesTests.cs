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
    public class AMillionCoincidencesTests : SimurghTestBase
    {
        [Test()]
        public void TestDamagesAllHeroes()
        {
            SetupSimurghGame();
            RemoveSimurghTriggers();

            QuickHPStorage(legacy, bunker, haka);

            PlayCard("AMillionCoincidences");

            // (H - 1) = 2 rounds of 1 projectile from the Simurgh plus
            // (H - 2) = 1 round of 1 irreducible melee from the environment.
            QuickHPCheck(-3, -3, -3);
        }
    }
}
