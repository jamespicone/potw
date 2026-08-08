using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Lung
{
    [TestFixture()]
    public class LungTests : LungTestBase
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.Lung", "Tempest", "InsulaPrimalis");
        }

        [Test()]
        public void TestSetup()
        {
            SetupLungGame();

            AssertHitPoints(lung.CharacterCard, 80);
            AssertIsInPlay(brute);
            AssertNotFlipped(lung.CharacterCard);
            AssertNotFlipped(brute);
        }

        [Test()]
        public void TestEndOfTurnPlaysTopCardAndDealsDamage()
        {
            SetupLungGame();

            // Note Lung doesn't play a villain card during the play phase; the only play
            // is Brute's end-of-turn effect.
            var wings = StackDeck("Wings");

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageType(DamageType.Melee, DamageType.Melee, DamageType.Melee);
            AssertDamageSource(lung.CharacterCard, lung.CharacterCard, lung.CharacterCard);

            GoToEndOfTurn();

            // Brute played the top card...
            AssertIsInPlay(wings);

            // ... and Lung dealt 1 + 0 / 5 = 1 melee damage to each hero target.
            QuickHPCheck(-1, -1, -1);
        }

        [Test()]
        public void TestEndOfTurnDamageScalesWithTrashAtFive()
        {
            SetupLungGame();
            FillLungTrash(5);
            SetHitPoints(lung.CharacterCard, 40);

            StackDeck("Wings");

            QuickHPStorage(legacy.CharacterCard, bunker.CharacterCard, haka.CharacterCard, lung.CharacterCard);
            GoToEndOfTurn();

            // X = 1 + 5 / 5 = 2, and Lung regains 1 HP for having 5+ cards in the trash.
            QuickHPCheck(-2, -2, -2, 1);
        }

        [Test()]
        public void TestEndOfTurnDamageRoundsDown()
        {
            SetupLungGame();
            FillLungTrash(9);
            SetHitPoints(lung.CharacterCard, 40);

            StackDeck("Wings");

            QuickHPStorage(legacy.CharacterCard, bunker.CharacterCard, haka.CharacterCard, lung.CharacterCard);
            GoToEndOfTurn();

            // X = 1 + 9 / 5 = 2 (rounds down), regain 1 HP for 5+.
            QuickHPCheck(-2, -2, -2, 1);
        }

        [Test()]
        public void TestEndOfTurnDamageAndDoubleRegenAtFifteen()
        {
            SetupLungGame();
            FillLungTrash(15);
            SetHitPoints(lung.CharacterCard, 40);

            StackDeck("Wings");

            QuickHPStorage(legacy.CharacterCard, bunker.CharacterCard, haka.CharacterCard, lung.CharacterCard);
            GoToEndOfTurn();

            // X = 1 + 15 / 5 = 4, and Lung regains 1 HP twice (5+ and 15+).
            QuickHPCheck(-4, -4, -4, 2);
        }

        [Test()]
        public void TestFirstDamageReducedWithTenInTrash()
        {
            SetupLungGame();
            FillLungTrash(10);

            QuickHPStorage(lung);
            DealDamage(haka, lung, 5, DamageType.Melee);
            QuickHPCheck(-4);

            // Second damage in the same round is not reduced.
            DealDamage(haka, lung, 5, DamageType.Melee);
            QuickHPCheck(-5);
        }

        [Test()]
        public void TestDamageReductionResetsEachRound()
        {
            SetupLungGame();
            RemoveEnvironmentDeck();
            FillLungTrash(10);

            QuickHPStorage(lung);
            DealDamage(haka, lung, 5, DamageType.Melee);
            QuickHPCheck(-4);

            StackDeck("Wings");
            GoToEndOfTurn();
            GoToStartOfTurn(lung);

            QuickHPStorage(lung);
            DealDamage(haka, lung, 5, DamageType.Melee);
            QuickHPCheck(-4);
        }

        [Test()]
        public void TestNoDamageReductionWithNineInTrash()
        {
            SetupLungGame();
            FillLungTrash(9);

            QuickHPStorage(lung);
            DealDamage(haka, lung, 5, DamageType.Melee);
            QuickHPCheck(-5);
        }

        [Test()]
        public void TestFlipsWhenVillainDeckEmpty()
        {
            SetupLungGame();

            var pyro = PlayCard("Pyrokinesis");
            MoveCards(lung, lung.TurnTaker.Deck.Cards.ToList(), lung.TurnTaker.Trash);

            GoToEndOfTurn();

            AssertFlipped(lung.CharacterCard);
            AssertFlipped(brute);

            // All noncharacter villain cards in play are destroyed and the deck is emptied into the trash.
            AssertInTrash(pyro);
            AssertNumberOfCardsInDeck(lung, 0);
        }

        [Test()]
        public void TestFlippedReducesDamageByOne()
        {
            SetupLungGame();
            MoveCards(lung, lung.TurnTaker.Deck.Cards.ToList(), lung.TurnTaker.Trash);
            GoToEndOfTurn();
            AssertFlipped(lung.CharacterCard);

            QuickHPStorage(lung);
            DealDamage(haka, lung, 5, DamageType.Melee);
            QuickHPCheck(-4);

            // Flat reduction, not once per round.
            DealDamage(haka, lung, 5, DamageType.Melee);
            QuickHPCheck(-4);
        }

        [Test()]
        public void TestFlippedVillainCardsCannotBePlayed()
        {
            SetupLungGame();
            MoveCards(lung, lung.TurnTaker.Deck.Cards.ToList(), lung.TurnTaker.Trash);
            GoToEndOfTurn();
            AssertFlipped(lung.CharacterCard);

            QuickHPStorage(legacy, bunker, haka);

            var smash = GetCardFromTrash(lung, "Smash");
            PlayCard(smash);

            AssertInTrash(smash);
            QuickHPCheck(0, 0, 0);
        }

        [Test()]
        public void TestFlippedImmuneToEnvironmentDamage()
        {
            SetupLungGame();
            var pileup = PlayCard("TrafficPileup");
            MoveCards(lung, lung.TurnTaker.Deck.Cards.ToList(), lung.TurnTaker.Trash);
            GoToEndOfTurn();
            AssertFlipped(lung.CharacterCard);

            QuickHPStorage(lung);
            DealDamage(pileup, lung.CharacterCard, 4, DamageType.Melee);
            QuickHPCheck(0);

            // Still takes (reduced) damage from heroes.
            DealDamage(haka, lung, 4, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestFlippedEndOfTurn()
        {
            SetupLungGame();

            var presence = PlayCard("InspiringPresence");
            var ammo = PlayCard("AmmoDrop");

            SetHitPoints(lung.CharacterCard, 40);
            MoveCards(lung, lung.TurnTaker.Deck.Cards.ToList(), lung.TurnTaker.Trash);

            QuickHPStorage(legacy.CharacterCard, bunker.CharacterCard, haka.CharacterCard, lung.CharacterCard);

            // The flip happens during Brute's end-of-turn play, and the flipped side's
            // end-of-turn triggers then fire in the same phase: 6 melee and 2 irreducible
            // fire to each hero, Lung regains 3 HP, and H - 1 = 2 hero ongoing/equipment
            // cards are destroyed. No decisions are set: the damage targets auto-select,
            // and presence/ammo are the only two destroyable cards.
            GoToEndOfTurn();
            AssertFlipped(lung.CharacterCard);

            QuickHPCheck(-8, -8, -8, 3);
            AssertInTrash(presence);
            AssertInTrash(ammo);
        }

        [Test()]
        public void TestAdvancedDiscardsTopCardAtEndOfTurn()
        {
            SetupLungGame(advanced: true);

            // Two harmless ongoings: one for Brute's end-of-turn play and one for the
            // advanced discard (order of the end-of-turn triggers doesn't matter).
            StackDeckHandleDuplicates("Wings", "WhiteHotFlame");

            GoToEndOfTurn();

            AssertNumberOfCardsInTrash(lung, 1);
        }

        [Test()]
        public void TestFlippedAdvancedHeroesLose()
        {
            SetupLungGame(advanced: true);

            FlipCard(lung.CharacterCard);

            AssertGameOver(EndingResult.AlternateDefeat);
        }

        [Test()]
        public void TestAdvancedFlipsWhenDiscardReshufflesTrash()
        {
            // With the deck empty, the advanced end-of-turn discard reshuffles the
            // trash into the deck; that reshuffle flips Lung, which in advanced
            // mode means the heroes lose.
            SetupLungGame(advanced: true);

            MoveCards(lung, lung.TurnTaker.Deck.Cards.ToList(), lung.TurnTaker.Trash);

            GoToEndOfTurn();

            AssertFlipped(lung.CharacterCard);
            AssertGameOver(EndingResult.AlternateDefeat);
        }
    }
}
