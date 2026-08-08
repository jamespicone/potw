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
    public class YangbanNewDelhiTests : NewDelhiTestBase
    {
        [Test()]
        public void TestEndOfTurnFourTypedDamages()
        {
            SetupNewDelhiGame();

            var yangban = PlayCard("YangbanNewDelhi");

            // Baron Blade (40) stays the highest target throughout.
            QuickHPStorage(baron.CharacterCard);
            AssertDamageType(DamageType.Fire, DamageType.Cold, DamageType.Lightning, DamageType.Energy);
            AssertDamageSource(yangban, yangban, yangban, yangban);

            GoToEndOfTurn(env);

            QuickHPCheck(-4);
        }
    }
}
