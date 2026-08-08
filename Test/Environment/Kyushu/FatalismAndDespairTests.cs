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
    public class FatalismAndDespairTests : KyushuTestBase
    {
        [Test()]
        public void TestEachHeroDealsSelfPsychic()
        {
            SetupKyushuGame();

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageType(DamageType.Psychic);

            PlayCard("FatalismAndDespair", 0);

            QuickHPCheck(-2, -2, -2);
        }

        [Test()]
        public void TestSelfDestructsAtEndOfTurn()
        {
            SetupKyushuGame();

            var card = PlayCard("FatalismAndDespair", 0);

            GoToEndOfTurn(env);

            AssertInTrash(card);
        }
    }
}
