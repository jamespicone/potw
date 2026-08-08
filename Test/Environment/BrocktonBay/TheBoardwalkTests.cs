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
    public class TheBoardwalkTests : BrocktonBayTestBase
    {
        [Test()]
        public void TestReducesDamageToEnvironmentTargets()
        {
            SetupBrocktonBayGame();

            PlayCard("TheBoardwalk");
            var squad = PlayCard("PRTSquad");

            QuickHPStorage(squad);
            DealDamage(haka, squad, 3, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestEnteringPlayDestroysOtherSuburbs()
        {
            SetupBrocktonBayGame();

            var docks = PlayCard("TheDocks");
            var boardwalk = PlayCard("TheBoardwalk");

            AssertInTrash(docks);
            AssertIsInPlay(boardwalk);
        }
    }
}
