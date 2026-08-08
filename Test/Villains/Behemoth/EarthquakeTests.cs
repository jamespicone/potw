using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Behemoth
{
    [TestFixture()]
    public class EarthquakeTests : BehemothTestBase
    {
        [Test()]
        public void TestDestroysCardsAndDamagesHeroes()
        {
            SetupBehemothGame();
            RemoveBehemothTriggers();
            ClearProximity();

            var pileup = PlayCard("TrafficPileup");     // environment
            var sense = PlayCard("DangerSense");        // ongoing
            var flak = PlayCard("FlakCannon");          // equipment

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageType(DamageType.Melee, DamageType.Melee, DamageType.Melee);

            PlayCard("Earthquake");

            AssertInTrash(pileup);
            AssertInTrash(sense);
            AssertInTrash(flak);

            QuickHPCheck(-2, -2, -2);
        }
    }
}
