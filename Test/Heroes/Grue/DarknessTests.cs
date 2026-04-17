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
    public class DarknessTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "InsulaPrimalis");

            StartGame();
        }

        [Test()]
        public void TestDarknessIsNextToTarget()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "InsulaPrimalis");

            StartGame();

            var platform = GetMobileDefensePlatform();
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(platform.Card));

            var darkness = platform.Card.GetAllNextToCards(false).FirstOrDefault(c => c.Identifier == "Darkness");
            Assert.That(darkness, Is.Not.Null, "Darkness should be next to the target");
            AssertNextToCard(darkness, platform.Card);
        }

        [Test()]
        public void TestDarknessFallsOffWhenTargetDestroyed()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "InsulaPrimalis");

            StartGame();

            var platform = GetMobileDefensePlatform();
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(platform.Card));

            AssertIsInPlay(platform.Card);
            AssertNumberOfCardsNextToCard(platform.Card, 1);

            var darkness = platform.Card.GetAllNextToCards(false).FirstOrDefault();
            Assert.That(darkness.Identifier, Is.EqualTo("Darkness"));

            DestroyCard(platform.Card);

            AssertInTrash(platform.Card);

            AssertNumberOfCardsNextToCard(platform.Card, 0);

            AssertIsInPlay(darkness);
        }

        [Test()]
        public void TestDarknessGoesToOutOfGame()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "InsulaPrimalis");

            StartGame();

            RemoveVillainTriggers();

            var platform = GetMobileDefensePlatform();
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(platform.Card));

            AssertIsInPlay(platform.Card);
            AssertNumberOfCardsNextToCard(platform.Card, 1);

            var darkness = platform.Card.GetAllNextToCards(false).FirstOrDefault();
            Assert.That(darkness.Identifier, Is.EqualTo("Darkness"));

            DestroyCard(darkness);

            AssertIsInPlay(platform.Card);

            AssertNumberOfCardsNextToCard(platform.Card, 0);

            AssertOutOfGame(darkness);
        }

        [Test()]
        public void TestDarknessLeavesAtEndOfNextTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "InsulaPrimalis");

            StartGame();

            RemoveVillainTriggers();

            GoToUsePowerPhase(grue);

            var platform = GetMobileDefensePlatform();
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(platform.Card));

            AssertIsInPlay(platform.Card);
            AssertNumberOfCardsNextToCard(platform.Card, 1);

            var darkness = platform.Card.GetAllNextToCards(false).FirstOrDefault();
            Assert.That(darkness.Identifier, Is.EqualTo("Darkness"));

            GoToStartOfTurn(grue);

            AssertIsInPlay(platform.Card);
            AssertNumberOfCardsNextToCard(platform.Card, 1);
            AssertIsInPlay(darkness);

            GoToDrawCardPhase(grue);

            AssertIsInPlay(platform.Card);
            AssertNumberOfCardsNextToCard(platform.Card, 1);
            AssertIsInPlay(darkness);

            GoToEndOfTurn(grue);

            AssertOutOfGame(darkness);
        }

        [Test()]
        public void TestReducesFirstDamageDealtByTarget()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            // Put Darkness next to Baron
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(baron.CharacterCard));

            // Baron deals damage - should be reduced by 1
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Melee);
            QuickHPCheck(-2); // 3 - 1 = 2
        }

        [Test()]
        public void TestReducesFirstDamageDealtToTarget()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            // Put Darkness next to Bunker
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(bunker.CharacterCard));

            // Baron deals damage to Bunker - should be reduced by 1
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Melee);
            QuickHPCheck(-2); // 3 - 1 = 2
        }

        [Test()]
        public void TestReductionDoesNotApplyToGrueDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            // Put Darkness next to Grue (simulating he dealt damage from adjacent darkness)
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(grue.CharacterCard));

            // Grue deals damage - should NOT be reduced
            QuickHPStorage(baron);
            DealDamage(grue, baron, 3, DamageType.Melee);
            QuickHPCheck(-3); // Full 3 damage, not reduced
        }

        [Test()]
        public void TestSourceReductionResetsEachTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();
            RemoveVillainTriggers();

            // Put Darkness next to Baron
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(baron.CharacterCard));

            // Baron's first damage is reduced
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Melee);
            QuickHPCheck(-2);

            // Go to next turn
            GoToStartOfTurn(grue);

            // Baron's first damage is reduced again
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestTargetReductionResetsEachTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();
            RemoveVillainTriggers();

            // Put Darkness next to Bunker
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(bunker.CharacterCard));

            // First damage to Bunker is reduced
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Melee);
            QuickHPCheck(-2);

            // Go to next turn
            GoToStartOfTurn(grue);

            // First damage to Bunker is reduced again
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestSecondDamageDealtByTargetNotReduced()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            // Put Darkness next to Baron
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(baron.CharacterCard));

            // Baron's first damage is reduced
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Melee);
            QuickHPCheck(-2);

            // Baron's second damage is not reduced
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestSecondDamageDealtToTargetNotReduced()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            // Put Darkness next to Bunker
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(bunker.CharacterCard));

            // First damage to Bunker is reduced
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Melee);
            QuickHPCheck(-2);

            // Second damage to Bunker is not reduced
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestMultipleDarknessOnDifferentTargets()
        {
            // Use Absolute Zero instead of Legacy since Legacy is Baron's nemesis (+1 damage)
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "AbsoluteZero", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            // Put Darkness next to Bunker and Absolute Zero
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(bunker.CharacterCard));
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(az.CharacterCard));

            // First damage to Bunker is reduced
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Melee);
            QuickHPCheck(-2);

            // First damage to Absolute Zero is reduced
            QuickHPStorage(az);
            DealDamage(baron, az, 3, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestDarknessRetrievedFromOutOfGame()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            // Find the darkness card and move it out of game
            var darkness = GetCard("Darkness");
            MoveCard(grue, darkness, grue.TurnTaker.OutOfGame);
            AssertOutOfGame(darkness);

            // Put Darkness into play - should retrieve from out of game
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(bunker.CharacterCard));

            AssertIsInPlay(darkness);
            AssertNextToCard(darkness, bunker.CharacterCard);
        }

        [Test()]
        public void TestNewDarknessSynthesized()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "Legacy", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();
            RemoveVillainTriggers();

            // Use power twice in different turns to create multiple Darkness cards
            DecisionSelectCard = bunker.CharacterCard;
            UsePower(grue.CharacterCard);

            // The first use creates 2 Darkness cards (one next to Grue, one next to Bunker)
            var darknessCards = FindCardsWhere(c => c.Identifier == "Darkness" && c.IsInPlay);
            Assert.That(darknessCards.Count(), Is.EqualTo(2), "Should have 2 Darkness cards in play");
        }

        [Test()]
        public void TestDarknessStaysInPlayWhenTargetMoves()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var platform = GetMobileDefensePlatform();
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(platform.Card));

            var darkness = platform.Card.GetAllNextToCards(false).FirstOrDefault(c => c.Identifier == "Darkness");
            Assert.That(darkness, Is.Not.Null);

            DestroyCard(platform.Card);

            // Darkness should still be in play (in play area)
            AssertIsInPlay(darkness);
        }

        [Test()]
        public void TestBothReductionsCanApply()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            // Put Darkness next to both Baron and Bunker
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(baron.CharacterCard));
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(bunker.CharacterCard));

            // Baron deals damage to Bunker - both reductions should apply
            // -1 from Baron's Darkness (first damage dealt BY)
            // -1 from Bunker's Darkness (first damage dealt TO)
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 4, DamageType.Melee);
            QuickHPCheck(-2); // 4 - 1 - 1 = 2
        }

        [Test()]
        public void TestDarknessDoesNotReduceDamageFromGrueCharacter()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            // Put Darkness next to Baron (target of damage)
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(baron.CharacterCard));

            // Grue deals damage to Baron - Grue's damage is completely exempt from Darkness reduction
            QuickHPStorage(baron);
            DealDamage(grue, baron, 3, DamageType.Melee);
            QuickHPCheck(-3); // No reduction for Grue's damage
        }

        [Test()]
        public void TestDarknessOnGrueDoesNotReduceGrueDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            // Put Darkness next to Grue
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(grue.CharacterCard));

            // Grue deals damage - Darkness next to Grue should NOT reduce it
            QuickHPStorage(baron);
            DealDamage(grue, baron, 3, DamageType.Melee);
            QuickHPCheck(-3); // Full damage
        }
    }
}
