using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Dragon
{
    [TestFixture()]
    public class SabotageTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            // Play an ongoing to destroy
            var livingForceField = PlayCard("LivingForceField");

            DecisionSelectCard = livingForceField;
            var card = PlayCard("Sabotage");
            AssertInTrash(card); // One-shot goes to trash
        }

        [Test()]
        public void TestDestroysVillainOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var villainOngoing = PlayCard("LivingForceField");
            AssertIsInPlay(villainOngoing);

            DecisionSelectCard = villainOngoing;
            PlayCard("Sabotage");

            AssertInTrash(villainOngoing);
        }

        [Test()]
        public void TestDestroysHeroOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Legacy", "InsulaPrimalis");
            StartGame();

            var heroOngoing = PlayCard("NextEvolution");
            AssertIsInPlay(heroOngoing);

            DecisionSelectCard = heroOngoing;
            PlayCard("Sabotage");

            AssertInTrash(heroOngoing);
        }

        [Test()]
        public void TestCanDestroyAnyOngoing()
        {
            // Sabotage can destroy any ongoing, including Dragon's own ongoings
            // This is tested by TestDestroysDragonOngoing, TestDestroysVillainOngoing, TestDestroysHeroOngoing
            // InsulaPrimalis doesn't have environment ongoing cards, so we skip testing that
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Legacy", "InsulaPrimalis");
            StartGame();

            // Play multiple ongoings
            var villainOngoing = PlayCard("LivingForceField");
            var heroOngoing = PlayCard("NextEvolution");
            var dragonOngoing = PlayCard("Archives");

            // Can select any of them
            DecisionSelectCard = villainOngoing;
            PlayCard("Sabotage");

            // Villain ongoing was destroyed
            AssertInTrash(villainOngoing);
            // Others still in play
            AssertIsInPlay(heroOngoing);
            AssertIsInPlay(dragonOngoing);
        }

        [Test()]
        public void TestIsOneShot()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var card = GetCard("Sabotage");

            Assert.That(card.IsOneShot, Is.True, "Sabotage should be a One-Shot");
        }

        [Test()]
        public void TestDestroysDragonOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var dragonOngoing = PlayCard("Archives");
            AssertIsInPlay(dragonOngoing);

            DecisionSelectCard = dragonOngoing;
            PlayCard("Sabotage");

            AssertInTrash(dragonOngoing);
        }
    }
}
