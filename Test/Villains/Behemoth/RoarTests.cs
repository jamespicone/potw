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
    public class RoarTests : BehemothTestBase
    {
        [Test()]
        public void TestDamagesAllNonVillainTargets()
        {
            SetupBehemothGame();
            RemoveBehemothTriggers();
            ClearProximity();

            var pileup = PlayCard("TrafficPileup");

            QuickHPStorage(legacy.CharacterCard, bunker.CharacterCard, haka.CharacterCard, pileup);
            AssertDamageType(DamageType.Sonic, DamageType.Sonic, DamageType.Sonic, DamageType.Sonic);

            PlayCard("Roar");

            QuickHPCheck(-2, -2, -2, -2);
        }
    }
}
