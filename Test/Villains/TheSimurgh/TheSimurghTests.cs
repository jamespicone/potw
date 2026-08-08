using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.TheSimurgh
{
    [TestFixture()]
    public class TheSimurghTests : SimurghTestBase
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheSimurgh", "Tempest", "InsulaPrimalis");
        }

        [Test()]
        public void TestSetup()
        {
            SetupSimurghGame();

            AssertHitPoints(simurgh.CharacterCard, 50);
            AssertNotFlipped(simurgh.CharacterCard);

            // 4 traps face down in play, 3 off to the side.
            Assert.That(FaceDownTrapsInPlay.Count(), Is.EqualTo(4));
            Assert.That(
                FindCardsWhere(c => c.DoKeywordsContain("trap") && c.Location.IsOffToTheSide).Count(),
                Is.EqualTo(3));

            // The start of villain turn 1 already played one of the 5 conditions.
            Assert.That(ConditionDeck.NumberOfCards, Is.EqualTo(4));
        }

        [Test()]
        public void TestReducesDamageByOne()
        {
            SetupSimurghGame();

            QuickHPStorage(simurgh);
            DealDamage(haka, simurgh, 4, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestFaceDownCardsIndestructible()
        {
            SetupSimurghGame();

            var trap = FaceDownTrapsInPlay.First();
            DestroyCard(trap);

            AssertIsInPlay(trap);
        }

        [Test()]
        public void TestEndOfTurnDamagesHighestHero()
        {
            SetupSimurghGame();

            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 15);
            SetHitPoints(haka, 25);

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageType(DamageType.Projectile);
            AssertDamageSource(simurgh.CharacterCard);

            GoToEndOfTurn();

            // H - 1 = 2 projectile to the highest hero.
            QuickHPCheck(0, 0, -2);
        }

        [Test()]
        public void TestStartOfTurnPlaysConditionCard()
        {
            SetupSimurghGame();
            RemoveCountermeasures();

            // An Unfortunate Malfunction fires with H = 3 equipment cards in play.
            var malfunction = GetCard("AnUnfortunateMalfunction");
            MoveCard(simurgh, malfunction, ConditionDeck);

            PlayCard("FlakCannon");
            PlayCard("GatlingGun");
            PlayCard("HeavyPlating");

            var trap = PutTrapFaceDownInPlay("ADefencePenetrated");
            foreach (var other in FaceDownTrapsInPlay.Where(c => c != trap).ToList())
            {
                FlipCard(other);
            }

            GoToEndOfTurn();
            GoToStartOfTurn(simurgh);

            Assert.That(trap.IsFlipped, Is.False, "The condition should have flipped the trap face up");
            AssertOutOfGame(malfunction);
        }

        [Test()]
        public void TestFlipsWhenNoFaceDownCards()
        {
            SetupSimurghGame();
            RemoveCountermeasures();

            foreach (var trap in FaceDownTrapsInPlay.ToList())
            {
                FlipCard(trap);
            }

            GoToEndOfTurn();

            AssertFlipped(simurgh.CharacterCard);
        }

        [Test()]
        public void TestNoFlipWhileFaceDownCardsRemain()
        {
            SetupSimurghGame();

            GoToEndOfTurn();

            AssertNotFlipped(simurgh.CharacterCard);
        }

        [Test()]
        public void TestFlippedReducesDamageByTwo()
        {
            SetupSimurghGame();

            FlipCard(simurgh.CharacterCard);

            QuickHPStorage(simurgh);
            DealDamage(haka, simurgh, 4, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestFlippedScreamDamage()
        {
            SetupSimurghGame();

            FlipCard(simurgh.CharacterCard);

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageType(DamageType.Psychic, DamageType.Psychic, DamageType.Psychic);

            GoToEndOfTurn();

            // One Scream token: 1 psychic to each hero target.
            QuickHPCheck(-1, -1, -1);
            Assert.That(simurgh.CharacterCard.FindTokenPool("ScreamPool").CurrentValue, Is.EqualTo(1));
        }

        [Test()]
        public void TestFlippedAdvancedPutsTrapIntoPlay()
        {
            SetupSimurghGame(advanced: true);
            RemoveCountermeasures();

            FlipCard(simurgh.CharacterCard);

            // Choose a trap with a harmless when-flipped effect.
            var offSideTrap = FindCardsWhere(
                c => c.DoKeywordsContain("trap") && c.Location.IsOffToTheSide).FirstOrDefault();
            Assert.That(offSideTrap, Is.Not.Null);
            DecisionSelectCard = offSideTrap;

            GoToEndOfTurn();

            AssertIsInPlay(offSideTrap);
        }
    }
}
