using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;
using Jp.ParahumansOfTheWormverse.Grue;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Grue
{
    [TestFixture()]
    public class SpewDarknessTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "InsulaPrimalis");

            StartGame();
        }

        [Test()]
        public void TestOnPlay_GrueDeals3PsychicToSelf()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            DecisionSelectCard = grue.CharacterCard;

            QuickHPStorage(grue);
            PlayCard("SpewDarkness");
            QuickHPCheck(-3); // 3 psychic self-damage
        }

        [Test()]
        public void TestHeroesWithDarknessImmune()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            // Put Darkness next to Bunker first
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(bunker.CharacterCard));

            DecisionSelectCard = grue.CharacterCard;
            PlayCard("SpewDarkness");

            // Now Bunker with Darkness should be immune to non-hero damage
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 5, DamageType.Melee);
            QuickHPCheck(0); // Immune
        }

        [Test()]
        public void TestOnlyImmuneToNonHeroDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            // Put Darkness next to Bunker first
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(bunker.CharacterCard));

            DecisionSelectCard = grue.CharacterCard;
            PlayCard("SpewDarkness");

            // Hero damage still works
            QuickHPStorage(bunker);
            DealDamage(grue, bunker, 2, DamageType.Melee);
            QuickHPCheck(-2); // Takes damage from hero
        }

        [Test()]
        public void TestRequiresDarknessAdjacent()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            DecisionSelectCard = grue.CharacterCard;
            PlayCard("SpewDarkness");

            // Bunker does NOT have Darkness next to him
            // So he should NOT be immune
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 5, DamageType.Melee);
            QuickHPCheck(-5); // Not immune
        }

        [Test()]
        public void TestDestroysAtStartOfTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveVillainTriggers();

            DecisionSelectCard = grue.CharacterCard;
            var spew = PlayCard("SpewDarkness");
            AssertIsInPlay(spew);

            GoToStartOfTurn(grue);

            AssertInTrash(spew);
        }

        [Test()]
        public void TestNotLimited()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            DecisionSelectCard = grue.CharacterCard;

            var spew1 = PlayCard("SpewDarkness", 0);
            AssertIsInPlay(spew1);

            var spew2 = PlayCard("SpewDarkness", 1);
            AssertIsInPlay(spew2);
            AssertIsInPlay(spew1); // Still in play, not Limited
        }

        [Test()]
        public void TestDamageIsPsychic()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            DecisionSelectCard = grue.CharacterCard;

            AssertDamageType(DamageType.Psychic);
            PlayCard("SpewDarkness");
        }

        [Test()]
        public void TestDamageSourceIsGrue()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            DecisionSelectCard = grue.CharacterCard;

            AssertDamageSource(grue.CharacterCard);
            PlayCard("SpewDarkness");
        }

        [Test()]
        public void TestGrueWithDarknessIsImmune()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            // Put Darkness next to Grue first
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(grue.CharacterCard));

            DecisionSelectCard = bunker.CharacterCard;
            PlayCard("SpewDarkness");

            // Now Grue with Darkness should be immune to non-hero damage
            QuickHPStorage(grue);
            DealDamage(baron, grue, 5, DamageType.Melee);
            QuickHPCheck(0); // Immune
        }

        [Test()]
        public void TestPlacesDarknessNextToHeroOnPlay()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            DecisionSelectCard = bunker.CharacterCard;
            PlayCard("SpewDarkness");

            // Should have placed Darkness next to Bunker
            var darknessNextToBunker = bunker.CharacterCard.GetAllNextToCards(false).Where(c => c.Identifier == "Darkness");
            Assert.That(darknessNextToBunker.Count(), Is.EqualTo(1), "Should have 1 Darkness next to Bunker");
        }

        [Test()]
        public void TestIsOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var spew = GetCard("SpewDarkness");

            Assert.That(spew.DoKeywordsContain("ongoing"), Is.True, "Spew Darkness should be ongoing");
        }
    }
}
