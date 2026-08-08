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
    public class ACountermeasureDefeatedTests : SimurghTestBase
    {
        [Test()]
        public void TestPlaysLowestDangerCardFromTrash()
        {
            SetupSimurghGame();
            RemoveSimurghTriggers();

            // Trash: A Million Coincidences has the lowest SimurghDanger (2).
            var coincidences = PutInTrash("AMillionCoincidences");
            PutInTrash("AFateSelected");
            PutInTrash("AnEnvironmentChosen");
            RemoveEnvironmentDeck(); // so A Million Coincidences' environment damage source is clean

            QuickHPStorage(legacy, bunker, haka);

            PlayCard("ACountermeasureDefeated");

            // A Million Coincidences was played: (H - 1) x 1 projectile plus
            // (H - 2) x 1 irreducible melee to each hero target = 3 total each.
            QuickHPCheck(-3, -3, -3);
            AssertInTrash(coincidences);
        }
    }
}
