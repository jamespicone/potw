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
    public class LungKyushuTests : KyushuTestBase
    {
        [Test()]
        public void TestReducesDamageToSelf()
        {
            SetupKyushuGame();

            var lung = PlayCard("LungKyushu");

            QuickHPStorage(lung);
            DealDamage(haka, lung, 3, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestEndOfTurnFireDamageToAllOtherTargets()
        {
            SetupKyushuGame();

            var lung = PlayCard("LungKyushu");

            QuickHPStorage(baron.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, haka.CharacterCard, lung);
            AssertDamageType(DamageType.Fire);

            GoToEndOfTurn(env);

            // 3 fire to every target other than itself.
            QuickHPCheck(-3, -3, -3, -3, 0);
        }

        [Test()]
        public void TestStartOfTurnRegen()
        {
            SetupKyushuGame();

            var lung = PlayCard("LungKyushu");
            SetHitPoints(lung, 5);

            GoToStartOfTurn(env);

            AssertHitPoints(lung, 7);
        }
    }
}
