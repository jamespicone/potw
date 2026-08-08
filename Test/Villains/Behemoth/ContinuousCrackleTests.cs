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
    public class ContinuousCrackleTests : BehemothTestBase
    {
        [Test()]
        public void TestHeroOngoingEnteringPlayTriggersDamage()
        {
            SetupBehemothGame();
            RemoveBehemothTriggers();
            ClearProximity();

            PlayCard("ContinuousCrackle");

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageType(DamageType.Lightning);
            AssertDamageSource(behemoth.CharacterCard);

            PlayCard("DangerSense");

            QuickHPCheck(-2, 0, 0);
        }

        [Test()]
        public void TestHeroEquipmentEnteringPlayTriggersDamage()
        {
            SetupBehemothGame();
            RemoveBehemothTriggers();
            ClearProximity();

            PlayCard("ContinuousCrackle");

            QuickHPStorage(legacy, bunker, haka);

            PlayCard("FlakCannon");

            QuickHPCheck(0, -2, 0);
        }
    }
}
