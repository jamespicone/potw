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
    public class BurstTests : ParahumanTest
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

            var card = GetCard("Burst");
            Assert.That(card.IsOneShot, Is.True);
        }

        [Test()]
        public void TestBurstAllowsMultipleEffects()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();
            RemoveVillainCards();
            RemoveVillainTriggers();

            // Play two effect-providing laser cards
            var freezeblast = PlayCard("Freezeblast");
            var scatterblast = PlayCard("Scatterblast");

            GoToPlayCardPhase(legend);

            // Play Burst to enable multi-effect selection
            PlayCard("Burst");

            // Use Legend's power - with Burst active, can choose all 3 effects
            // Character card effect (2 energy) + Freezeblast (1 cold) + Scatterblast (5-1=4 energy)
            // Loop ends when no more unique effects remain
            DecisionActivateAbilities = new Card[] { legend.CharacterCard, freezeblast, scatterblast };
            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            UsePower(legend);
            // 2 energy + 1 cold + 4 energy = 7 total
            QuickHPCheck(-7);
        }

        [Test()]
        public void TestBurstAddsStatusEffect()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
            StartGame();

            var statusCountBefore = GameController.StatusEffectManager.StatusEffectControllers.Count();

            GoToPlayCardPhase(legend);
            PlayCard("Burst");

            var statusCountAfter = GameController.StatusEffectManager.StatusEffectControllers.Count();
            Assert.That(statusCountAfter, Is.GreaterThan(statusCountBefore));
        }
    }
}
