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
    public class AThreatForeseenTests : SimurghTestBase
    {
        [Test()]
        public void TestRedirectsFirstDamageEachRound()
        {
            SetupSimurghGame();
            RemoveSimurghTriggers();

            PlayCard("AThreatForeseen");

            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 10);
            SetHitPoints(haka, 25);

            QuickHPStorage(simurgh.CharacterCard, bunker.CharacterCard);

            // First damage redirected to the lowest hero.
            DealDamage(haka, simurgh, 3, DamageType.Melee);
            QuickHPCheck(0, -3);

            // Second damage sticks (note the Simurgh's damage reduction was
            // removed along with her triggers).
            DealDamage(haka, simurgh, 3, DamageType.Melee);
            QuickHPCheck(-3, 0);
        }
    }
}
