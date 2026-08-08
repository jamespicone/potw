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
    public class LightningRodTests : NewDelhiTestBase
    {
        [Test()]
        public void TestRedirectsLightningDamage()
        {
            SetupNewDelhiGame();

            var rod = PlayCard("LightningRod");

            QuickHPStorage(bunker.CharacterCard, rod);

            DealDamage(haka, bunker, 3, DamageType.Lightning);

            QuickHPCheck(0, -3);
        }

        [Test()]
        public void TestDoesNotRedirectOtherDamage()
        {
            SetupNewDelhiGame();

            var rod = PlayCard("LightningRod");

            QuickHPStorage(bunker.CharacterCard, rod);

            DealDamage(haka, bunker, 3, DamageType.Melee);

            QuickHPCheck(-3, 0);
        }
    }
}
