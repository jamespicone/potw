using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Leviathan
{
    [TestFixture()]
    public class WhippingTailTests : LeviathanTestBase
    {
        [Test()]
        public void TestDamagesTwoLowestHeroes()
        {
            SetupLeviathanGame();
            RemoveVillainTriggers();
            MoveTacticsToDeckBottom();

            SetHitPoints(legacy, 15);
            SetHitPoints(bunker, 16);
            SetHitPoints(haka, 30);

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageType(DamageType.Melee, DamageType.Melee);
            AssertDamageSource(leviathan.CharacterCard, leviathan.CharacterCard);

            PlayCard("WhippingTail");

            QuickHPCheck(-2, -2, 0);
        }
    }
}
