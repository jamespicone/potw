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
    public class DischargeTests : BehemothTestBase
    {
        [Test()]
        public void TestDamagesAllHeroTargets()
        {
            SetupBehemothGame();
            RemoveBehemothTriggers();
            ClearProximity();

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageType(DamageType.Energy, DamageType.Energy, DamageType.Energy);
            AssertDamageSource(behemoth.CharacterCard, behemoth.CharacterCard, behemoth.CharacterCard);

            PlayCard("Discharge");

            QuickHPCheck(-2, -2, -2);
        }
    }
}
