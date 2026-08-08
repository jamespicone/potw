using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.TheSimurgh
{
    [TestFixture()]
    public class AFateSelectedTests : SimurghTestBase
    {
        [Test()]
        public void TestDamagesHeroWithFewestCardsInPlay()
        {
            SetupSimurghGame();
            RemoveSimurghTriggers();

            // Legacy and Haka each have a card in play; Bunker has none.
            PlayCard("DangerSense");
            PlayCard("Dominion");

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageType(DamageType.Sonic);
            AssertDamageSource(simurgh.CharacterCard);

            PlayCard("AFateSelected");

            // H = 3 sonic damage.
            QuickHPCheck(0, -3, 0);
        }
    }
}
