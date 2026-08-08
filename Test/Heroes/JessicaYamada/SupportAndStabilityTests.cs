using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.JessicaYamada
{
    [TestFixture()]
    public class SupportAndStabilityTests : ParahumanTest
    {
        [Test()]
        public void TestMayPreventFirstSelfDamageEachTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Legacy", "Megalopolis");
            StartGame();
            RemoveVillainCards();
            RemoveVillainTriggers();

            PlayCard("SupportAndStability");

            QuickHPStorage(legacy);
            DecisionYesNo = true;

            DealDamage(legacy, legacy, 3, DamageType.Melee);
            QuickHPCheck(0);

            // Second self-damage in the same turn is not prevented.
            DealDamage(legacy, legacy, 3, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestMayDeclineToPrevent()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Legacy", "Megalopolis");
            StartGame();
            RemoveVillainCards();
            RemoveVillainTriggers();

            PlayCard("SupportAndStability");

            QuickHPStorage(legacy);
            DecisionYesNo = false;

            DealDamage(legacy, legacy, 3, DamageType.Melee);
            QuickHPCheck(-3);
        }
    }
}
