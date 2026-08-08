using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Environment.NewDelhi
{
    [TestFixture()]
    public class CracklingLightningTests : NewDelhiTestBase
    {
        [Test()]
        public void TestDamagesAllHeroesOnEnter()
        {
            SetupNewDelhiGame();

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageType(DamageType.Lightning);

            PlayCard("CracklingLightning");

            QuickHPCheck(-3, -3, -3);
        }

        [Test()]
        public void TestSelfDestructsAtEndOfTurn()
        {
            SetupNewDelhiGame();

            var lightning = PlayCard("CracklingLightning");

            GoToEndOfTurn(env);

            AssertInTrash(lightning);
        }
    }
}
