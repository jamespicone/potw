using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Behemoth
{
    [TestFixture()]
    public class BehemothTests : BehemothTestBase
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.Behemoth", "Tempest", "InsulaPrimalis");
        }

        [Test()]
        public void TestSetup()
        {
            SetupBehemothGame();

            AssertHitPoints(behemoth.CharacterCard, 100);
            AssertNotFlipped(behemoth.CharacterCard);
            AssertIsInPlay(heroTactics);
            AssertNotFlipped(heroTactics);

            // Each hero has a proximity marker...
            Assert.That(Proximity(legacy), Is.Not.Null);
            Assert.That(Proximity(bunker), Is.Not.Null);
            Assert.That(Proximity(haka), Is.Not.Null);

            // ... and the 10 movement cards are split between the Movement deck and trash
            // (one was already played at the start of villain turn 1).
            Assert.That(MovementDeck.NumberOfCards + MovementTrashPile.NumberOfCards, Is.EqualTo(10));
        }

        [Test()]
        public void TestEndOfTurnProximityDamage()
        {
            SetupBehemothGame();

            SetProximity(legacy, 1);
            SetProximity(bunker, 2);
            SetProximity(haka, 3);

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageType(DamageType.Psychic, DamageType.Psychic, DamageType.Psychic);
            AssertDamageSource(behemoth.CharacterCard, behemoth.CharacterCard, behemoth.CharacterCard);

            GoToEndOfTurn();

            QuickHPCheck(-1, -2, -3);
        }

        [Test()]
        public void TestDamageMatchesCurrentType()
        {
            SetupBehemothGame();

            // Behemoth's starting damage type is psychic; cold damage he deals is converted.
            QuickHPStorage(haka);
            AssertDamageType(DamageType.Psychic);

            DealDamage(behemoth.CharacterCard, haka.CharacterCard, 2, DamageType.Cold);

            QuickHPCheck(-2);
        }

        [Test()]
        public void TestPlaysMovementCardAtStartOfTurn()
        {
            SetupBehemothGame();
            ClearProximity();

            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 20);
            SetHitPoints(haka, 30);

            var focus = StackMovementDeck("Focus");

            GoToEndOfTurn();
            GoToStartOfTurn(behemoth);

            // Focus was played: the hero with the highest HP gained a proximity token.
            Assert.That(Proximity(haka).CurrentValue, Is.EqualTo(1));
            Assert.That(Proximity(legacy).CurrentValue, Is.EqualTo(0));
            Assert.That(Proximity(bunker).CurrentValue, Is.EqualTo(0));

            // The used movement card went to the movement trash.
            Assert.That(focus.Location, Is.EqualTo(MovementTrashPile));
        }

        [Test()]
        public void TestFlipsBelowThirtyHP()
        {
            SetupBehemothGame();
            ClearProximity();

            SetHitPoints(behemoth.CharacterCard, 29);

            GoToEndOfTurn();
            GoToStartOfTurn(behemoth);

            AssertFlipped(behemoth.CharacterCard);
            AssertFlipped(heroTactics);
        }

        [Test()]
        public void TestNoFlipAtThirtyHP()
        {
            SetupBehemothGame();
            ClearProximity();

            SetHitPoints(behemoth.CharacterCard, 30);

            GoToEndOfTurn();
            GoToStartOfTurn(behemoth);

            AssertNotFlipped(behemoth.CharacterCard);
            AssertNotFlipped(heroTactics);
        }

        [Test()]
        public void TestFlippedRedirectsMatchingDamage()
        {
            SetupBehemothGame();
            ClearProximity();

            FlipCard(behemoth.CharacterCard);

            SetHitPoints(bunker, 10); // lowest hero target

            // Damage of Behemoth's current type (psychic) is redirected to the lowest hero.
            QuickHPStorage(behemoth.CharacterCard, bunker.CharacterCard);
            DealDamage(haka, behemoth, 3, DamageType.Psychic);
            QuickHPCheck(0, -3);

            // Damage of any other type goes through.
            DealDamage(haka, behemoth, 3, DamageType.Cold);
            QuickHPCheck(-3, 0);
        }

        [Test()]
        public void TestFlippedPlaysVillainCardAtEndOfTurn()
        {
            SetupBehemothGame();
            ClearProximity();

            FlipCard(behemoth.CharacterCard);

            var juggernaut = StackDeck("Juggernaut");

            GoToEndOfTurn();

            AssertIsInPlay(juggernaut);
        }

        [Test()]
        public void TestAdvancedIncapacitatesAtSixTokens()
        {
            SetupBehemothGame(advanced: true);
            ClearProximity();

            SetProximity(bunker, 5);

            // Advance adds a token to each hero, pushing Bunker to 6.
            PlayMovementCard("Advance");

            AssertIncapacitated(bunker);
        }
    }
}
