using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Dauntless
{
    [TestFixture()]
    public class CarryTheChargeTests : ParahumanTest
    {
        [Test()]
        public void TestDrawOnDamageTaken()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            PlayCard("CarryTheCharge");

            QuickHandStorage(dauntless);
            DecisionYesNo = true; // Yes, draw a card
            DealDamage(baron, dauntless, 2, DamageType.Melee);

            QuickHandCheck(1); // Drew 1 card
        }

        [Test()]
        public void TestDrawIsOptional()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            PlayCard("CarryTheCharge");

            // Turn off autodraw so we can actually decline
            GameController.PlayerPolicies.AutoDrawCards = AlwaysSmartNever.Never;

            QuickHandStorage(dauntless);
            DecisionYesNo = false; // No, don't draw a card
            DealDamage(baron, dauntless, 2, DamageType.Melee);

            QuickHandCheck(0); // No draw because we declined
        }

        [Test()]
        public void TestPlayOnEnergyDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            PlayCard("CarryTheCharge");

            // Put a card in hand to play
            var corona = PutInHand("CracklingCorona");

            DecisionYesNo = true; // Yes, draw a card
            DecisionSelectCardToPlay = corona;
            DealDamage(baron, dauntless, 2, DamageType.Energy);

            // Corona should have been played (moved to trash as one-shot)
            AssertInTrash(corona);
        }

        [Test()]
        public void TestPlayOnLightningDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            PlayCard("CarryTheCharge");

            // Put a card in hand to play
            var corona = PutInHand("CracklingCorona");

            DecisionYesNo = true; // Yes, draw a card
            DecisionSelectCardToPlay = corona;
            DealDamage(baron, dauntless, 2, DamageType.Lightning);

            // Corona should have been played (moved to trash as one-shot)
            AssertInTrash(corona);
        }

        [Test()]
        public void TestPlayIsOptional()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            PlayCard("CarryTheCharge");

            // Put a card in hand
            var corona = PutInHand("CracklingCorona");

            DecisionYesNo = true; // Yes, draw a card
            DecisionDoNotSelectCard = SelectionType.PlayCard; // Don't play a card
            DealDamage(baron, dauntless, 2, DamageType.Energy);

            // Corona should still be in hand (we declined to play)
            AssertInHand(corona);
        }

        [Test()]
        public void TestNoPlayOnMeleeDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            PlayCard("CarryTheCharge");

            var corona = PutInHand("CracklingCorona");
            int initialHandCount = GetNumberOfCardsInHand(dauntless);

            DecisionYesNo = true;
            DealDamage(baron, dauntless, 2, DamageType.Melee);

            AssertInHand(corona);
            Assert.That(GetNumberOfCardsInHand(dauntless), Is.EqualTo(initialHandCount + 1));
        }

        [Test()]
        public void TestNoPlayOnFireDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            PlayCard("CarryTheCharge");

            var corona = PutInHand("CracklingCorona");
            int initialHandCount = GetNumberOfCardsInHand(dauntless);

            DecisionYesNo = true;
            DealDamage(baron, dauntless, 2, DamageType.Fire);

            AssertInHand(corona);
            Assert.That(GetNumberOfCardsInHand(dauntless), Is.EqualTo(initialHandCount + 1));
        }

        [Test()]
        public void TestNoPlayOnColdDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            PlayCard("CarryTheCharge");

            var corona = PutInHand("CracklingCorona");
            int initialHandCount = GetNumberOfCardsInHand(dauntless);

            DecisionYesNo = true;
            DealDamage(baron, dauntless, 2, DamageType.Cold);

            AssertInHand(corona);
            Assert.That(GetNumberOfCardsInHand(dauntless), Is.EqualTo(initialHandCount + 1));
        }

        [Test()]
        public void TestNoPlayOnPsychicDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            PlayCard("CarryTheCharge");

            var corona = PutInHand("CracklingCorona");
            int initialHandCount = GetNumberOfCardsInHand(dauntless);

            DecisionYesNo = true;
            DealDamage(baron, dauntless, 2, DamageType.Psychic);

            AssertInHand(corona);
            Assert.That(GetNumberOfCardsInHand(dauntless), Is.EqualTo(initialHandCount + 1));
        }

        [Test()]
        public void TestNoPlayOnToxicDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            PlayCard("CarryTheCharge");

            var corona = PutInHand("CracklingCorona");
            int initialHandCount = GetNumberOfCardsInHand(dauntless);

            DecisionYesNo = true;
            DealDamage(baron, dauntless, 2, DamageType.Toxic);

            AssertInHand(corona);
            Assert.That(GetNumberOfCardsInHand(dauntless), Is.EqualTo(initialHandCount + 1));
        }

        [Test()]
        public void TestNoPlayOnRadiantDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            PlayCard("CarryTheCharge");

            var corona = PutInHand("CracklingCorona");
            int initialHandCount = GetNumberOfCardsInHand(dauntless);

            DecisionYesNo = true;
            DealDamage(baron, dauntless, 2, DamageType.Radiant);

            AssertInHand(corona);
            Assert.That(GetNumberOfCardsInHand(dauntless), Is.EqualTo(initialHandCount + 1));
        }

        [Test()]
        public void TestNoPlayOnInfernalDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            PlayCard("CarryTheCharge");

            var corona = PutInHand("CracklingCorona");
            int initialHandCount = GetNumberOfCardsInHand(dauntless);

            DecisionYesNo = true;
            DealDamage(baron, dauntless, 2, DamageType.Infernal);

            AssertInHand(corona);
            Assert.That(GetNumberOfCardsInHand(dauntless), Is.EqualTo(initialHandCount + 1));
        }

        [Test()]
        public void TestNoPlayOnProjectileDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            PlayCard("CarryTheCharge");

            var corona = PutInHand("CracklingCorona");
            int initialHandCount = GetNumberOfCardsInHand(dauntless);

            DecisionYesNo = true;
            DealDamage(baron, dauntless, 2, DamageType.Projectile);

            AssertInHand(corona);
            Assert.That(GetNumberOfCardsInHand(dauntless), Is.EqualTo(initialHandCount + 1));
        }

        [Test()]
        public void TestNoPlayOnSonicDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            PlayCard("CarryTheCharge");

            var corona = PutInHand("CracklingCorona");
            int initialHandCount = GetNumberOfCardsInHand(dauntless);

            DecisionYesNo = true;
            DealDamage(baron, dauntless, 2, DamageType.Sonic);

            AssertInHand(corona);
            Assert.That(GetNumberOfCardsInHand(dauntless), Is.EqualTo(initialHandCount + 1));
        }

        [Test()]
        public void TestNoTriggerIfNoDamageDealt()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            PlayCard("CarryTheCharge");

            // Put Arcshield into play and use its power to give Dauntless damage reduction
            var arcshield = PlayCard("Arcshield");
            UsePower(arcshield);

            QuickHandStorage(dauntless);
            // Deal 1 damage which is reduced by 1 to 0
            DealDamage(baron, dauntless, 1, DamageType.Melee);

            QuickHandCheck(0); // No draw because no damage was dealt
        }

        [Test()]
        public void TestOnlyTriggersOnDamageToDauntless()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Legacy", "InsulaPrimalis");
            StartGame();

            PlayCard("CarryTheCharge");

            QuickHandStorage(dauntless);
            DealDamage(baron, legacy, 2, DamageType.Melee);

            QuickHandCheck(0); // No draw because damage was to Legacy
        }

        [Test()]
        public void TestLimited()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var carryTheCharge1 = PutInHand("CarryTheCharge");
            var carryTheCharge2 = PutInHand("CarryTheCharge");

            PlayCard(carryTheCharge1);
            AssertIsInPlay(carryTheCharge1);

            PlayCard(carryTheCharge2);

            // Second copy goes to trash (limited prevents it from entering play)
            AssertIsInPlay(carryTheCharge1);
            AssertInTrash(carryTheCharge2);
        }

        [Test()]
        public void TestTriggersForEachInstanceOfDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            PlayCard("CarryTheCharge");

            QuickHandStorage(dauntless);
            DecisionsYesNo = new bool[] { true, true };

            // Deal damage twice
            DealDamage(baron, dauntless, 1, DamageType.Melee);
            DealDamage(baron, dauntless, 1, DamageType.Melee);

            QuickHandCheck(2); // Drew 2 cards
        }
    }
}
