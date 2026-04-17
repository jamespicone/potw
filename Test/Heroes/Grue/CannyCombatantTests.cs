using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Grue
{
    [TestFixture()]
    public class CannyCombatantTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "InsulaPrimalis");

            StartGame();
        }

        [Test()]
        public void TestBaseDamageIs2()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            // Destroy any ongoings that might have started in play
            var ongoings = FindCardsWhere(c => c.DoKeywordsContain("ongoing") && c.IsInPlay);
            foreach (var og in ongoings)
            {
                DestroyCard(og);
            }

            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            PlayCard("CannyCombatant");
            QuickHPCheck(-2); // 2 + 0 ongoings = 2
        }

        [Test()]
        public void TestScalesWithOngoings()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            // Destroy any ongoings that might have started in play
            var ongoings = FindCardsWhere(c => c.DoKeywordsContain("ongoing") && c.IsInPlay).ToList();
            foreach (var og in ongoings)
            {
                DestroyCard(og);
            }

            // Play 2 ongoings
            PlayCard("Leadership");
            PlayCard("MartialTalent");

            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            PlayCard("CannyCombatant");
            // Base: 2 + 2 ongoings = 4
            // Leadership: +1
            // MartialTalent: +1 (melee damage)
            // Total: 6
            QuickHPCheck(-6);
        }

        [Test()]
        public void TestCountsAllOngoings()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            // Destroy any ongoings that might have started in play
            var ongoings = FindCardsWhere(c => c.DoKeywordsContain("ongoing") && c.IsInPlay).ToList();
            foreach (var og in ongoings)
            {
                DestroyCard(og);
            }

            // Play hero ongoing
            PlayCard("Leadership");

            // Play villain ongoing
            var villainOngoing = PlayCard("LivingForceField");

            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            PlayCard("CannyCombatant");
            // Base: 2 + 2 ongoings = 4
            // Leadership: +1
            // Living Force Field: -1
            // Total: 4
            QuickHPCheck(-4);
        }

        [Test()]
        public void TestDamageSourceIsGrue()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            DecisionSelectTarget = baron.CharacterCard;

            AssertDamageSource(grue.CharacterCard);
            PlayCard("CannyCombatant");
        }

        [Test()]
        public void TestDamageTypeIsMelee()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            DecisionSelectTarget = baron.CharacterCard;

            AssertDamageType(DamageType.Melee);
            PlayCard("CannyCombatant");
        }

        [Test()]
        public void TestOnlyCountsOngoingsInPlay()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            // Put Leadership in trash, not in play
            var leadership = PutInTrash("Leadership");

            // Destroy any ongoings that might have started in play
            var ongoings = FindCardsWhere(c => c.DoKeywordsContain("ongoing") && c.IsInPlay).ToList();
            foreach (var og in ongoings)
            {
                DestroyCard(og);
            }

            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            PlayCard("CannyCombatant");
            QuickHPCheck(-2); // 2 + 0 ongoings in play = 2
        }

        [Test()]
        public void TestIsOneShot()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var card = GetCard("CannyCombatant");

            Assert.That(card.DoKeywordsContain("one-shot"), Is.True, "Canny Combatant should be a one-shot");
        }

        [Test()]
        public void TestCanTargetAnyTarget()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            // All targets should be valid
            AssertNextDecisionChoices(
                included: new Card[] { baron.CharacterCard, bunker.CharacterCard, grue.CharacterCard }
            );

            DecisionSelectTarget = baron.CharacterCard;
            PlayCard("CannyCombatant");
        }
    }
}
