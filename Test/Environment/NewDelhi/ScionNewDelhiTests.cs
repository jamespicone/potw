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
    public class ScionNewDelhiTests : NewDelhiTestBase
    {
        [Test()]
        public void TestEverythingImmuneAndIndestructible()
        {
            SetupNewDelhiGame();

            var fortitude = PlayCard("Fortitude");
            PlayCard("ScionNewDelhi");

            // All targets are immune to damage.
            QuickHPStorage(bunker);
            DealDamage(haka, bunker, 3, DamageType.Melee);
            QuickHPCheck(0);

            // All cards are indestructible.
            DestroyCard(fortitude);
            AssertIsInPlay(fortitude);
        }

        [Test()]
        public void TestRemovesSelfAtStartOfTurn()
        {
            SetupNewDelhiGame();

            var scion = PlayCard("ScionNewDelhi");

            GoToStartOfTurn(env);

            AssertOutOfGame(scion);
        }
    }
}
