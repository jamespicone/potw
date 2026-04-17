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
    public class ScattershotTests : ParahumanTest
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

            var card = GetCard("Scattershot");
            Assert.That(card.DoKeywordsContain("laser"), Is.True);
            Assert.That(card.DoKeywordsContain("ongoing"), Is.True);
        }

        [Test()]
        public void TestPowerHitsAllTargetsExceptLegend()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var scattershot = PlayCard("Scattershot");

            // Character card effect auto-selects (only effect available)
            // Scattershot targets all targets except Legend automatically

            QuickHPStorage(baron.CharacterCard, bunker.CharacterCard, legend.CharacterCard);
            UsePower(scattershot);
            // Baron and Bunker take 2, Legend takes 0
            QuickHPCheck(-2, -2, 0);
        }

        [Test()]
        public void TestPowerDoesNotHitLegend()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var scattershot = PlayCard("Scattershot");

            QuickHPStorage(legend);
            UsePower(scattershot);
            // Legend should not be damaged
            QuickHPCheck(0);
        }
    }
}
