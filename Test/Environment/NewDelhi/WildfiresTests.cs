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
    public class WildfiresTests : NewDelhiTestBase
    {
        [Test()]
        public void TestEndOfTurnDamagesAllTargets()
        {
            SetupNewDelhiGame();

            PlayCard("Wildfires");

            QuickHPStorage(baron.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, haka.CharacterCard);
            AssertDamageType(DamageType.Fire);

            GoToEndOfTurn(env);

            QuickHPCheck(-2, -2, -2, -2);
        }

        [Test()]
        public void TestDestroyedByColdDamage()
        {
            SetupNewDelhiGame();

            var wildfires = PlayCard("Wildfires");

            DealDamage(haka, bunker, 1, DamageType.Cold);

            AssertInTrash(wildfires);
        }
    }
}
