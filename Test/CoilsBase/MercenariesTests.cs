using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Jp.ParahumansOfTheWormverse.Tattletale;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.CoilsBase
{
    [TestFixture()]
    public class MercenariesTests : ParahumanTest
    {
        [Test()]
        public void TestRegular()
        {
            SetupGameController(
                "BaronBlade",
                "Legacy",
                "Tachyon",
                "Jp.ParahumansOfTheWormverse.CoilsBase"
            );

            var mercs = PlayCard("Mercenaries");

            SetHitPoints(baron, 40);
            SetHitPoints(legacy, 30);
            SetHitPoints(tachyon, 20);
            SetHitPoints(mercs, 5);

            // Will shoot baron and legacy for 1 energy damage
            QuickHPStorage(baron.CharacterCard, legacy.CharacterCard, tachyon.CharacterCard, mercs);

            GoToEndOfTurn(FindEnvironment());

            QuickHPCheck(-1, -1, 0, 0);
        }

        [Test()]
        public void TestSkipsEnvironment()
        {
            SetupGameController(
                "BaronBlade",
                "Legacy",
                "Tachyon",
                "Jp.ParahumansOfTheWormverse.CoilsBase"
            );

            var mercs = PlayCard("Mercenaries");
            var chamber = PlayCard("SealedChamber");

            SetHitPoints(baron, 4);
            SetHitPoints(legacy, 3);
            SetHitPoints(tachyon, 2);
            SetHitPoints(mercs, 5);
            SetHitPoints(chamber, 10);

            // Will shoot baron and legacy for 1 energy damage
            QuickHPStorage(baron.CharacterCard, legacy.CharacterCard, tachyon.CharacterCard, mercs, chamber);

            GoToEndOfTurn(FindEnvironment());

            QuickHPCheck(-1, -1, 0, 0, 0);
        }
    }
}
