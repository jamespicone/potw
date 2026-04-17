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
    public class PreciseTargetingTests : ParahumanTest
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

            var card = GetCard("PreciseTargeting");
            Assert.That(card.IsOneShot, Is.True);
        }

        [Test()]
        public void TestDestroysOngoingCard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            var livingForceField = PlayCard("LivingForceField");
            AssertIsInPlay(livingForceField);

            DecisionSelectCard = livingForceField;
            PlayCard("PreciseTargeting");

            AssertInTrash(livingForceField);
        }

        [Test()]
        public void TestDestroysEnvironmentCard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
            StartGame();

            var envCard = PlayCard("ObsidianField");
            AssertIsInPlay(envCard);

            DecisionSelectCard = envCard;
            PlayCard("PreciseTargeting");

            AssertInTrash(envCard);
        }
    }
}
