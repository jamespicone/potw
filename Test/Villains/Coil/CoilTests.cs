using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Coil
{
    [TestFixture()]
    public class CoilTests : CoilTestBase
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.Coil", "Tempest", "InsulaPrimalis");
        }

        [Test()]
        public void TestSetup()
        {
            SetupCoilGame();

            AssertHitPoints(scheming, 50);
            AssertHitPoints(acting, 50);
            AssertIsInPlay(scheming);
            AssertIsInPlay(acting);
            AssertIsInPlay(GetCard("UndergroundBase"));
        }

        [Test()]
        public void TestSchemingEndOfTurnHealsVillains()
        {
            SetupCoilGame();
            CleanupSetupNoise();
            RemoveCardTriggers(acting);

            SetHitPoints(scheming, 40);
            SetHitPoints(acting, 45);

            GoToEndOfTurn();

            AssertHitPoints(scheming, 42);
            AssertHitPoints(acting, 47);
        }

        [Test()]
        public void TestSchemingMagicPlaysEnvironmentCard()
        {
            SetupCoilGame();
            CleanupSetupNoise();
            RemoveCardTriggers(acting);

            // Scheming's HP >= Acting's HP, so the magic text is active.
            SetHitPoints(acting, 40);

            var pileup = StackDeck("TrafficPileup");

            GoToEndOfTurn();
            GoToStartOfTurn(coil);

            AssertIsInPlay(pileup);
        }

        [Test()]
        public void TestSchemingNoMagicWhenLowerHP()
        {
            SetupCoilGame();
            CleanupSetupNoise();
            RemoveCardTriggers(acting);

            SetHitPoints(scheming, 30);

            var pileup = StackDeck("TrafficPileup");

            GoToEndOfTurn();
            GoToStartOfTurn(coil);

            AssertInDeck(pileup);
        }

        [Test()]
        public void TestActingMagicRevealsParahumanAndPutsItIntoPlay()
        {
            SetupCoilGame();
            CleanupSetupNoise();
            RemoveCardTriggers(scheming);

            var sundancer = StackDeck("Sundancer");

            GoToEndOfTurn();
            GoToStartOfTurn(coil);

            AssertIsInPlay(sundancer);
        }

        [Test()]
        public void TestActingMagicRevealsUpToTwoCards()
        {
            SetupCoilGame();
            CleanupSetupNoise();
            RemoveCardTriggers(scheming);

            // Two non-parahumans on top: both revealed (H - 1 = 2), neither played.
            var stacked = StackDeckHandleDuplicates("IKnowAllYourTricks", "TrickeryAndDeceit").ToList();

            GoToEndOfTurn();
            GoToStartOfTurn(coil);

            AssertInDeck(stacked[0]);
            AssertInDeck(stacked[1]);
        }

        [Test()]
        public void TestActingWithoutMagicRevealsOneCard()
        {
            SetupCoilGame();
            CleanupSetupNoise();
            RemoveCardTriggers(scheming);

            // Acting's HP < Scheming's: only 1 card revealed; the parahuman
            // underneath is never seen.
            SetHitPoints(acting, 40);
            var sundancer = StackDeck("Sundancer");
            StackDeck("IKnowAllYourTricks");

            GoToEndOfTurn();
            GoToStartOfTurn(coil);

            AssertInDeck(sundancer);
        }

        [Test()]
        public void TestActingEndOfTurnMagicDamage()
        {
            SetupCoilGame();
            CleanupSetupNoise();
            RemoveCardTriggers(scheming);

            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 15);
            SetHitPoints(haka, 25);

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageType(DamageType.Energy);
            AssertDamageSource(acting);

            // Acting's HP >= Scheming's: H = 3 damage to the highest hero.
            GoToEndOfTurn();

            QuickHPCheck(0, 0, -3);
        }

        [Test()]
        public void TestActingEndOfTurnDamageWithoutMagic()
        {
            SetupCoilGame();
            CleanupSetupNoise();
            RemoveCardTriggers(scheming);

            SetHitPoints(acting, 40);
            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 15);
            SetHitPoints(haka, 25);

            QuickHPStorage(legacy, bunker, haka);

            GoToEndOfTurn();

            QuickHPCheck(0, 0, -2);
        }

        [Test()]
        public void TestCoilFlipsInsteadOfBeingDestroyed()
        {
            SetupCoilGame();
            CleanupSetupNoise();

            DealDamage(haka, scheming, 60, DamageType.Melee);

            AssertFlipped(scheming);
            AssertIsInPlay(scheming);
            AssertNotGameOver();
        }

        [Test()]
        public void TestHeroesWinWhenBothCoilsDefeated()
        {
            SetupCoilGame();
            CleanupSetupNoise();

            DealDamage(haka, scheming, 60, DamageType.Melee);
            AssertNotGameOver();

            DealDamage(haka, acting, 60, DamageType.Melee);

            AssertGameOver();
        }

        [Test()]
        public void TestAdvancedEqualisesHPUpwards()
        {
            SetupCoilGame(advanced: true);
            CleanupSetupNoise();

            SetHitPoints(scheming, 30);

            GoToEndOfTurn();

            // Scheming healed 2 (its own end-of-turn), then was set to Acting's HP.
            AssertHitPoints(scheming, 50);
            AssertHitPoints(acting, 50);
        }

        [Test()]
        public void TestAdvancedFlippedCoilRevives()
        {
            SetupCoilGame(advanced: true);
            CleanupSetupNoise();

            SetHitPoints(acting, 37);
            DealDamage(haka, scheming, 60, DamageType.Melee);
            AssertFlipped(scheming);

            GoToEndOfTurn();

            // Advanced: the defeated side flips back with the other side's HP.
            AssertNotFlipped(scheming);
            AssertHitPoints(scheming, acting.HitPoints.Value);
        }
    }
}
