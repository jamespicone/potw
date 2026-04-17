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
    public class LuminalTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
        }

        [Test()]
        public void TestIsOneShot()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
            StartGame();

            var card = GetCard("Luminal");
            Assert.That(card.IsOneShot, Is.True);
        }

        [Test()]
        public void TestDraws3Cards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
            StartGame();

            QuickHandStorage(legend);
            PlayCard("Luminal");
            QuickHandCheck(3);
        }

        [Test()]
        public void TestGrantsAdditionalPowerUse()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var splitshot = PlayCard("Splitshot");

            GoToPlayCardPhase(legend);
            PlayCard("Luminal");

            // Use Legend's innate power
            DecisionSelectTarget = baron.CharacterCard;
            UsePower(legend);

            // Use Splitshot's power (second power use this turn)
            DecisionSelectTargets = new Card[] { baron.CharacterCard, null };
            UsePower(splitshot);
        }
    }
}
