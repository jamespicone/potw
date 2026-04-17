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
    public class SecondTriggerTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "InsulaPrimalis");

            StartGame();
        }

        [Test()]
        public void TestIncreasesDamageToGrueBy1()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("SecondTrigger");

            // Damage to Grue should be increased by 1
            QuickHPStorage(grue);
            DealDamage(baron, grue, 2, DamageType.Melee);
            QuickHPCheck(-3); // 2 + 1 = 3
        }

        [Test()]
        public void TestEnablesTriggerPowers()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            // Before Second Trigger
            Assert.That(grue.CharacterCardController.CanGrueUseTriggerPowers(), Is.False);

            PlayCard("SecondTrigger");

            // After Second Trigger
            Assert.That(grue.CharacterCardController.CanGrueUseTriggerPowers(), Is.True);
        }

        [Test()]
        public void TestRemovesSelfFromGame()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var secondTrigger = PutInHand("SecondTrigger");

            PlayCard(secondTrigger);

            AssertOutOfGame(secondTrigger);
        }

        [Test()]
        public void TestEffectPersistsGameLong()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();
            RemoveVillainTriggers();

            PlayCard("SecondTrigger");

            // Go through several turns
            GoToStartOfTurn(baron);
            GoToStartOfTurn(grue);
            GoToStartOfTurn(bunker);
            GoToStartOfTurn(env);
            GoToStartOfTurn(baron);

            // Effect should still be active
            QuickHPStorage(grue);
            DealDamage(baron, grue, 2, DamageType.Melee);
            QuickHPCheck(-3); // Still increased
        }

        [Test()]
        public void TestDamageIncreaseStacksWithOther()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("SecondTrigger");

            // Also play a card that increases damage
            PlayCard("Leadership"); // +1 to hero damage

            // Baron deals damage to Grue - only SecondTrigger applies since Leadership is hero damage
            QuickHPStorage(grue);
            DealDamage(baron, grue, 2, DamageType.Melee);
            QuickHPCheck(-3); // 2 + 1 = 3

            // Bunker deals damage to Grue - both SecondTrigger AND Leadership apply
            QuickHPStorage(grue);
            DealDamage(bunker, grue, 2, DamageType.Melee);
            QuickHPCheck(-4); // 2 + 1 (SecondTrigger) + 1 (Leadership) = 4
        }

        [Test()]
        public void TestTriggerPowerOnCharacterCard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("SecondTrigger");

            // The Trigger power (index 1) should now be usable
            DecisionSelectTurnTaker = bunker.TurnTaker;
            var handBefore = GetNumberOfCardsInHand(bunker);

            UsePower(grue.CharacterCard, 1);

            // Bunker's power draws a card
            var handAfter = GetNumberOfCardsInHand(bunker);
            Assert.That(handAfter, Is.EqualTo(handBefore + 1));
        }

        [Test()]
        public void TestCardNotInTrash()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var secondTrigger = PutInHand("SecondTrigger");

            PlayCard(secondTrigger);

            // Card should NOT be in trash
            AssertNotInTrash(secondTrigger);
            AssertOutOfGame(secondTrigger);
        }

        [Test()]
        public void TestOnlyOneExistsInDeck()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var secondTriggerCards = FindCardsWhere(c => c.Identifier == "SecondTrigger" && c.Owner == grue.TurnTaker);
            Assert.That(secondTriggerCards.Count(), Is.EqualTo(1), "There should only be 1 Second Trigger card in Grue's deck");
        }

        [Test()]
        public void TestDoesNotAffectOtherHeroes()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("SecondTrigger");

            // Damage to Bunker should NOT be increased
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 2, DamageType.Melee);
            QuickHPCheck(-2); // Not increased, just 2
        }

        [Test()]
        public void TestIsOneShot()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var secondTrigger = GetCard("SecondTrigger");

            Assert.That(secondTrigger.DoKeywordsContain("one-shot"), Is.True, "Second Trigger should be a one-shot");
        }
    }
}
