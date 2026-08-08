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
    public class ChevalierNewDelhiTests : NewDelhiTestBase
    {
        [Test()]
        public void TestRedirectsHeroDamageToSelfWithReduction()
        {
            SetupNewDelhiGame();

            var chevalier = PlayCard("ChevalierNewDelhi");

            QuickHPStorage(bunker.CharacterCard, chevalier);

            DealDamage(haka, bunker, 3, DamageType.Melee);

            // Redirected to Chevalier, who reduces damage to himself by 1.
            QuickHPCheck(0, -2);
        }

        [Test()]
        public void TestDoesNotRedirectNonHeroDamage()
        {
            SetupNewDelhiGame();

            var chevalier = PlayCard("ChevalierNewDelhi");

            QuickHPStorage(baron.CharacterCard, chevalier);

            DealDamage(haka, baron, 3, DamageType.Melee);

            QuickHPCheck(-3, 0);
        }
    }
}
