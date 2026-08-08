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
    public class WildfireTests : BehemothTestBase
    {
        [Test()]
        public void TestDamagesHeroesWithThreeOrMoreTokens()
        {
            SetupBehemothGame();
            RemoveBehemothTriggers();
            ClearProximity();

            SetProximity(legacy, 3);
            SetProximity(bunker, 2);
            SetProximity(haka, 0);

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageType(DamageType.Fire);
            AssertDamageSource(behemoth.CharacterCard);

            PlayCard("Wildfire");

            // Only Legacy has 3+ proximity tokens; H = 3 damage.
            QuickHPCheck(-3, 0, 0);
        }

        [Test()]
        public void TestDamagesNonCharacterHeroTargets()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Behemoth", "Legacy", "Bunker", "TheVisionary", "Megalopolis");
            StartGame();
            RemoveBehemothTriggers();
            SetProximity(legacy, 0);
            SetProximity(bunker, 0);
            SetProximity(visionary, 0);

            var decoy = PlayCard("DecoyProjection");

            QuickHPStorage(legacy.CharacterCard, bunker.CharacterCard, visionary.CharacterCard, decoy);

            PlayCard("Wildfire");

            // No hero has 3+ tokens, so no character damage, but every noncharacter
            // hero target takes H = 3.
            QuickHPCheck(0, 0, 0, -3);
        }
    }
}
