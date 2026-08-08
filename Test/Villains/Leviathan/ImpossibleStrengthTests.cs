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
    public class ImpossibleStrengthTests : LeviathanTestBase
    {
        [Test()]
        public void TestIncreasesVillainDamage()
        {
            SetupLeviathanGame();
            RemoveVillainTriggers();
            MoveTacticsToDeckBottom();

            PlayCard("ImpossibleStrength");

            QuickHPStorage(haka);
            DealDamage(leviathan.CharacterCard, haka.CharacterCard, 2, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestDoesNotIncreaseHeroDamage()
        {
            SetupLeviathanGame();
            RemoveVillainTriggers();
            MoveTacticsToDeckBottom();

            PlayCard("ImpossibleStrength");

            QuickHPStorage(bunker);
            DealDamage(haka, bunker, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }
    }
}
