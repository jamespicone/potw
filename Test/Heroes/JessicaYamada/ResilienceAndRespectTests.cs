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
    public class ResilienceAndRespectTests : ParahumanTest
    {
        [Test()]
        public void TestReducesPsychicDamageToHeroes()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Legacy", "Megalopolis");
            StartGame();
            RemoveVillainCards();
            RemoveVillainTriggers();

            PlayCard("ResilienceAndRespect", 0);

            // Jessica's character card isn't a target, and Baron gets a nemesis
            // bonus against Legacy — so have Legacy hit himself.
            QuickHPStorage(legacy);
            DealDamage(legacy, legacy, 3, DamageType.Psychic);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestDoesNotReduceOtherDamageTypes()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Legacy", "Megalopolis");
            StartGame();
            RemoveVillainCards();
            RemoveVillainTriggers();

            PlayCard("ResilienceAndRespect", 0);

            QuickHPStorage(legacy);
            DealDamage(legacy, legacy, 3, DamageType.Melee);
            QuickHPCheck(-3);
        }
    }
}
