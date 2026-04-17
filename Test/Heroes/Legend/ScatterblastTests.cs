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
    public class ScatterblastTests : ParahumanTest
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

            var card = GetCard("Scatterblast");
            Assert.That(card.DoKeywordsContain("laser"), Is.True);
            Assert.That(card.DoKeywordsContain("ongoing"), Is.True);
        }

        [Test()]
        public void TestEffectDamageScalesWithSingleTarget()
        {
            // X = 5 - 1 target = 4 damage
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var scatterblast = PlayCard("Scatterblast");

            DecisionActivateAbilities = new Card[] { scatterblast };
            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            UsePower(legend);
            // 5 - 1 target = 4 damage
            QuickHPCheck(-4);
        }

        [Test()]
        public void TestEffectDamageScalesWithMultipleTargets()
        {
            // X = 5 - 2 targets = 3 damage each via Splitshot
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var bladeBattalion = PlayCard("BladeBattalion");
            var scatterblast = PlayCard("Scatterblast");
            var splitshot = PlayCard("Splitshot");

            DecisionActivateAbilities = new Card[] { scatterblast };
            DecisionSelectTargets = new Card[] { baron.CharacterCard, bladeBattalion, null };

            QuickHPStorage(baron.CharacterCard, bladeBattalion);
            UsePower(splitshot);
            // 5 - 2 targets = 3 damage each
            QuickHPCheck(-3, -3);
        }
    }
}
