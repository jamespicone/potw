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
    public class ImpossibleToughnessTests : LeviathanTestBase
    {
        [Test()]
        public void TestReducesDamageDealtToLeviathan()
        {
            SetupLeviathanGame();
            RemoveVillainTriggers();
            MoveTacticsToDeckBottom();

            PlayCard("ImpossibleToughness");

            QuickHPStorage(leviathan);
            DealDamage(haka, leviathan, 3, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestDoesNotReduceDamageToHeroes()
        {
            SetupLeviathanGame();
            RemoveVillainTriggers();
            MoveTacticsToDeckBottom();

            PlayCard("ImpossibleToughness");

            QuickHPStorage(haka);
            DealDamage(leviathan.CharacterCard, haka.CharacterCard, 3, DamageType.Melee);
            QuickHPCheck(-3);
        }
    }
}
