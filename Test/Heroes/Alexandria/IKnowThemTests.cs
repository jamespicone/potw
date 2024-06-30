using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Alexandria
{
    [TestFixture()]
    public class IKnowThemTests : ParahumanTest
    {
        [Test()]
        public void TestWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            var battalion = PlayCard("BladeBattalion");

            AssertNoDecision(SelectionType.SelectTarget);
            QuickHPStorage(baron.CharacterCard, battalion);
            PlayCard("IKnowThem");
            QuickHPCheck(-5, 0);
        }

        [Test()]
        public void TestBaronNotMostHP()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            var platform = PlayCard("MobileDefensePlatform");
            SetHitPoints(baron, 6);

            AssertNoDecision(SelectionType.SelectTarget);
            QuickHPStorage(baron.CharacterCard, platform);
            PlayCard("IKnowThem");
            QuickHPCheck(0, -5);
        }
    }
}
