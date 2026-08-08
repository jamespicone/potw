using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Environment.BrocktonBay
{
    [TestFixture()]
    public class TheDocksTests : BrocktonBayTestBase
    {
        [Test()]
        public void TestIncreasesEnvironmentDamage()
        {
            SetupBrocktonBayGame();

            PlayCard("TheDocks");
            PlayCard("PRTSquad");

            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 10);
            SetHitPoints(haka, 25);

            QuickHPStorage(bunker);

            GoToEndOfTurn(env);

            // The squad's 2 becomes 3 with The Docks in play.
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestDoesNotIncreaseHeroDamage()
        {
            SetupBrocktonBayGame();

            PlayCard("TheDocks");

            QuickHPStorage(bunker);
            DealDamage(haka, bunker, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }
    }
}
