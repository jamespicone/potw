using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Leviathan
{
    [TestFixture()]
    public class LeviathanTests : LeviathanTestBase
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.Leviathan", "Tempest", "InsulaPrimalis");
        }

        [Test()]
        public void TestSetup()
        {
            SetupLeviathanGame();

            AssertHitPoints(leviathan.CharacterCard, 100);
            AssertNotFlipped(leviathan.CharacterCard);

            // Setup reveals until a Tactic and puts it into play.
            AssertNumberOfCardsInPlay(c => c.DoKeywordsContain("tactic"), 1);
        }

        [Test()]
        public void TestEndOfTurnDamagesAllHeroes()
        {
            SetupLeviathanGame();
            MoveTacticsToDeckBottom();

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageType(DamageType.Melee, DamageType.Melee, DamageType.Melee);
            AssertDamageSource(leviathan.CharacterCard, leviathan.CharacterCard, leviathan.CharacterCard);

            GoToEndOfTurn();

            QuickHPCheck(-2, -2, -2);
        }

        [Test()]
        public void TestRetaliationTokenOnBigHit()
        {
            SetupLeviathanGame();
            MoveTacticsToDeckBottom();

            QuickTokenPoolStorage(RetaliationPool);

            // 8 - H = 5 damage in one hit earns a Retaliation token.
            DealDamage(haka, leviathan, 5, DamageType.Melee);

            QuickTokenPoolCheck(1);
        }

        [Test()]
        public void TestNoRetaliationTokenOnSmallHit()
        {
            SetupLeviathanGame();
            MoveTacticsToDeckBottom();

            QuickTokenPoolStorage(RetaliationPool);

            DealDamage(haka, leviathan, 4, DamageType.Melee);

            QuickTokenPoolCheck(0);
        }

        [Test()]
        public void TestFlipsAtEndOfTurnWithTokens()
        {
            SetupLeviathanGame();
            MoveTacticsToDeckBottom();
            var waterShadow = MoveTacticToDeckTop("WaterShadow");

            DealDamage(haka, leviathan, 5, DamageType.Melee);

            GoToEndOfTurn();

            AssertFlipped(leviathan.CharacterCard);
            Assert.That(RetaliationPool.CurrentValue, Is.EqualTo(0), "Retaliation tokens should have been removed");

            // Flipping to Retaliates reveals until a Tactic and puts it into play.
            AssertIsInPlay(waterShadow);
        }

        [Test()]
        public void TestNoFlipWithoutTokens()
        {
            SetupLeviathanGame();
            MoveTacticsToDeckBottom();

            GoToEndOfTurn();

            AssertNotFlipped(leviathan.CharacterCard);
        }

        [Test()]
        public void TestFlipsBackAtStartOfTurnAndDestroysHeroCards()
        {
            SetupLeviathanGame();
            MoveTacticsToDeckBottom();
            // Ensure the flip reveals a harmless tactic.
            MoveTacticToDeckTop("WaterShadow");
            RemoveEnvironmentDeck();

            var sense = PlayCard("DangerSense");
            var presence = PlayCard("InspiringPresence");
            var ammo = PlayCard("AmmoDrop");

            DealDamage(haka, leviathan, 5, DamageType.Melee);
            GoToEndOfTurn();
            AssertFlipped(leviathan.CharacterCard);

            // At the start of the next villain turn Leviathan flips back and destroys
            // H = 3 noncharacter hero cards (the only three in play).
            GoToStartOfTurn(leviathan);

            AssertNotFlipped(leviathan.CharacterCard);
            AssertInTrash(sense);
            AssertInTrash(presence);
            AssertInTrash(ammo);
        }

        [Test()]
        public void TestFlippedReducesDamageByOne()
        {
            SetupLeviathanGame();
            MoveTacticsToDeckBottom();
            // Ensure the flip reveals a harmless tactic.
            MoveTacticToDeckTop("WaterShadow");

            // Front side has no reduction.
            QuickHPStorage(leviathan);
            DealDamage(haka, leviathan, 3, DamageType.Melee);
            QuickHPCheck(-3);

            FlipCard(leviathan.CharacterCard);

            QuickHPStorage(leviathan);
            DealDamage(haka, leviathan, 3, DamageType.Melee);
            QuickHPCheck(-2);

            // Flat reduction, applies every time.
            DealDamage(haka, leviathan, 3, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestAdvancedReducesDamage()
        {
            SetupLeviathanGame(advanced: true);
            MoveTacticsToDeckBottom();

            QuickHPStorage(leviathan);
            DealDamage(haka, leviathan, 3, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestFlippedAdvancedPlaysCardAtEndOfTurn()
        {
            SetupLeviathanGame(advanced: true);
            MoveTacticsToDeckBottom();
            // Ensure the flip reveals a harmless tactic.
            MoveTacticToDeckTop("WaterShadow");

            FlipCard(leviathan.CharacterCard);
            AssertFlipped(leviathan.CharacterCard);

            var toughness = StackDeck("ImpossibleToughness");

            // Flipped advanced: at the end of the villain turn play the top card.
            GoToEndOfTurn();

            AssertIsInPlay(toughness);
        }
    }
}
