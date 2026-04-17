using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Legend
{
    [TestFixture()]
    public class SplitshotTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
        }

        [Test()]
        public void TestIsLaser()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
            StartGame();

            var card = GetCard("Splitshot");
            Assert.That(card.DoKeywordsContain("laser"), Is.True);
            Assert.That(card.DoKeywordsContain("ongoing"), Is.True);
        }

        [Test()]
        public void TestPowerHitsSingleTarget()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var splitshot = PlayCard("Splitshot");

            // Character card effect auto-selects; choose one target then stop
            DecisionSelectTargets = new Card[] { baron.CharacterCard, null };

            QuickHPStorage(baron);
            UsePower(splitshot);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestPowerHitsUpTo3Targets()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var bladeBattalion = PlayCard("BladeBattalion");
            var envTarget = PlayCard("VelociraptorPack");
            var splitshot = PlayCard("Splitshot");

            // Select 3 targets
            DecisionSelectTargets = new Card[] { baron.CharacterCard, bladeBattalion, envTarget, null };

            QuickHPStorage(baron.CharacterCard, bladeBattalion, envTarget);
            UsePower(splitshot);
            QuickHPCheck(-2, -2, -2);
        }

        [Test()]
        public void TestPowerCanSelectZeroTargets()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var splitshot = PlayCard("Splitshot");

            // Decline to select targets
            DecisionSelectTargets = new Card[] { null };

            QuickHPStorage(baron);
            UsePower(splitshot);
            QuickHPCheck(0);
        }
    }
}
