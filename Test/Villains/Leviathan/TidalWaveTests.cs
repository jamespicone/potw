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
    public class TidalWaveTests : LeviathanTestBase
    {
        [Test()]
        public void TestDestroysEnvironmentTargetsAndDealsDamage()
        {
            SetupLeviathanGame();
            RemoveVillainTriggers();
            MoveTacticsToDeckBottom();

            var pileup = PlayCard("TrafficPileup");
            var monorail = PlayCard("PlummetingMonorail");

            QuickHPStorage(legacy, bunker, haka);

            PlayCard("TidalWave");

            AssertInTrash(pileup);
            AssertInTrash(monorail);

            // X = 1 + 2 destroyed environment targets = 3 cold to all non-villain targets.
            QuickHPCheck(-3, -3, -3);
        }

        [Test()]
        public void TestDamageWithNoEnvironmentTargets()
        {
            SetupLeviathanGame();
            RemoveVillainTriggers();
            MoveTacticsToDeckBottom();

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageType(DamageType.Cold, DamageType.Cold, DamageType.Cold);
            AssertDamageSource(leviathan.CharacterCard, leviathan.CharacterCard, leviathan.CharacterCard);

            PlayCard("TidalWave");

            QuickHPCheck(-1, -1, -1);
        }
    }
}
