using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Environment.Kyushu
{
    [TestFixture()]
    public class SentaiEliteTests : KyushuTestBase
    {
        [Test()]
        public void TestDamagesHighestTarget()
        {
            SetupKyushuGame();

            var sentai = PlayCard("SentaiElite", 0);

            // Baron Blade (40) is the highest non-environment target.
            QuickHPStorage(baron.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, haka.CharacterCard);
            AssertDamageType(DamageType.Energy);
            AssertDamageSource(sentai);

            GoToEndOfTurn(env);

            // X = 2 x 1 Sentai Elite.
            QuickHPCheck(-2, 0, 0, 0);
        }

        [Test()]
        public void TestDamageScalesWithSentaiInPlay()
        {
            SetupKyushuGame();

            PlayCard("SentaiElite", 0);
            PlayCard("SentaiElite", 1);

            QuickHPStorage(baron.CharacterCard);

            GoToEndOfTurn(env);

            // Each of the two Sentai deals 2 x 2 = 4 to the highest target.
            QuickHPCheck(-8);
        }
    }
}
