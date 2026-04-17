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
    public class FillTheSkyTests : ParahumanTest
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

            var card = GetCard("FillTheSky");
            Assert.That(card.DoKeywordsContain("laser"), Is.True);
            Assert.That(card.DoKeywordsContain("ongoing"), Is.True);
        }

        [Test()]
        public void TestPowerDealsDamageAndDestroysItself()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var fillTheSky = PlayCard("FillTheSky");
            AssertIsInPlay(fillTheSky);

            // Character card's effect auto-selects; select Baron as the target, then stop
            DecisionSelectTargets = new Card[] { baron.CharacterCard, null };

            QuickHPStorage(baron);
            UsePower(fillTheSky);
            QuickHPCheck(-2);

            // Fill the Sky should be destroyed after use
            AssertInTrash(fillTheSky);
        }

        [Test()]
        public void TestPowerHitsMultipleTargets()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var bladeBattalion = PlayCard("BladeBattalion");
            var fillTheSky = PlayCard("FillTheSky");

            // Select both villain targets then stop
            DecisionSelectTargets = new Card[] { baron.CharacterCard, bladeBattalion, null };

            QuickHPStorage(baron.CharacterCard, bladeBattalion);
            UsePower(fillTheSky);
            QuickHPCheck(-2, -2);

            AssertInTrash(fillTheSky);
        }
    }
}
