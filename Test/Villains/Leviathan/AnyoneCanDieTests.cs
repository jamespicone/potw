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
    public class AnyoneCanDieTests : LeviathanTestBase
    {
        [Test()]
        public void TestDamagesHighestHeroIrreducibly()
        {
            SetupLeviathanGame();
            RemoveVillainTriggers();
            MoveTacticsToDeckBottom();

            PlayCard("Fortitude"); // Reduce damage dealt to Legacy by 1

            SetHitPoints(legacy, 30);
            SetHitPoints(bunker, 20);
            SetHitPoints(haka, 25);

            QuickHPStorage(legacy, bunker, haka);
            AssertIrreducible();
            AssertDamageType(DamageType.Melee);
            AssertDamageSource(leviathan.CharacterCard);

            PlayCard("AnyoneCanDie");

            // 5 irreducible melee to the highest hero; Fortitude can't reduce it.
            QuickHPCheck(-5, 0, 0);
        }
    }
}
