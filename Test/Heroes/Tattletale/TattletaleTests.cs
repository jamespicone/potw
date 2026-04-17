using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Tattletale
{
    [TestFixture()]
    public class TattletaleTests : ParahumanTest
    {
        #region Standard Variant

        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();
        }

        [Test()]
        public void TestHas26HP()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            Assert.That(tattletale.CharacterCard.MaximumHitPoints, Is.EqualTo(26));
        }

        [Test()]
        public void TestPowerDraws2Discards1()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            DecisionSelectTurnTaker = tattletale.TurnTaker;

            QuickHandStorage(tattletale);
            UsePower(tattletale);
            // Draw 2, discard 1 = net +1
            QuickHandCheck(1);
        }

        [Test()]
        public void TestPowerCanTargetOtherPlayer()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            DecisionSelectTurnTaker = bunker.TurnTaker;

            QuickHandStorage(bunker);
            UsePower(tattletale);
            // Draw 2, discard 1 = net +1
            QuickHandCheck(1);
        }

        [Test()]
        public void TestIncap0OnePlayerDraws()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            IncapacitateCharacter(tattletale.CharacterCard, baron.CharacterCard);

            QuickHandStorage(bunker);
            UseIncapacitatedAbility(tattletale, 0);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestIncap1OnePlayerPlays()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            IncapacitateCharacter(tattletale.CharacterCard, baron.CharacterCard);

            var card = PutInHand("FlakCannon");
            DecisionSelectCard = card;
            UseIncapacitatedAbility(tattletale, 1);
            AssertIsInPlay(card);
        }

        [Test()]
        public void TestIncap2DestroyOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            IncapacitateCharacter(tattletale.CharacterCard, baron.CharacterCard);

            var ongoing = PlayCard("LivingForceField");
            DecisionSelectCard = ongoing;
            UseIncapacitatedAbility(tattletale, 2);
            AssertInTrash(ongoing);
        }

        #endregion

        #region Ruler of Brockton Bay Variant

        [Test()]
        public void TestRulerModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale/TattletaleRulerOfBrocktonBayCharacter", "Bunker", "InsulaPrimalis");
            StartGame();
        }

        [Test()]
        public void TestRulerPowerUsesTwoPowers()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale/TattletaleRulerOfBrocktonBayCharacter", "Bunker", "InsulaPrimalis");
            StartGame();

            // Play two cards with powers
            var reading1 = PlayCard("Reading", 0);
            var reading2 = PlayCard("Reading", 1);

            DecisionSelectPowers = new Card[] { reading1, reading2 };
            DecisionSelectLocation = new LocationChoice(baron.TurnTaker.Deck);

            // Use character power which lets you use two powers
            UsePower(tattletale);
        }

        [Test()]
        public void TestRulerIncap0HeroUsesPower()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale/TattletaleRulerOfBrocktonBayCharacter", "Bunker", "InsulaPrimalis");
            StartGame();

            IncapacitateCharacter(tattletale.CharacterCard, baron.CharacterCard);

            // Bunker's innate power draws a card
            QuickHandStorage(bunker);
            UseIncapacitatedAbility(tattletale, 0);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestRulerIncap1RevealTopAndReplace()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale/TattletaleRulerOfBrocktonBayCharacter", "Bunker", "InsulaPrimalis");
            StartGame();

            IncapacitateCharacter(tattletale.CharacterCard, baron.CharacterCard);

            var topCard = baron.TurnTaker.Deck.TopCard;
            DecisionSelectLocation = new LocationChoice(baron.TurnTaker.Deck);
            UseIncapacitatedAbility(tattletale, 1);
            // Card should be replaced on top
            Assert.That(baron.TurnTaker.Deck.TopCard, Is.EqualTo(topCard));
        }

        #endregion

        #region Hunter of Secrets Variant

        [Test()]
        public void TestHunterModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale/TattletaleHunterOfSecretsCharacter", "Bunker", "InsulaPrimalis");
            StartGame();
        }

        [Test()]
        public void TestHunterPower0OngoingPlaysAndAddsToken()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale/TattletaleHunterOfSecretsCharacter", "Bunker", "InsulaPrimalis");
            StartGame();

            // Put an ongoing on top of deck
            var confidence = PutOnDeck("Confidence");

            UsePower(tattletale, 0);

            // Card should be in play
            AssertIsInPlay(confidence);
            // Token pool should have 1 token
            var pool = tattletale.CharacterCard.FindTokenPool("TattletaleHunterOfSecretsPool");
            Assert.That(pool.CurrentValue, Is.EqualTo(1));
        }

        [Test()]
        public void TestHunterPower0NonOngoingDiscards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale/TattletaleHunterOfSecretsCharacter", "Bunker", "InsulaPrimalis");
            StartGame();

            // Put a non-ongoing on top of deck
            var oneShot = PutOnDeck("TheReasonYouSuck");

            UsePower(tattletale, 0);

            // Card should be in trash
            AssertInTrash(oneShot);
        }

        [Test()]
        public void TestHunterPower0DuplicateOngoingDealsSelfDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale/TattletaleHunterOfSecretsCharacter", "Bunker", "InsulaPrimalis");
            StartGame();

            // Play an ongoing
            PlayCard("Confidence");

            // Add 2 tokens to pool first so X=2
            var pool = tattletale.CharacterCard.FindTokenPool("TattletaleHunterOfSecretsPool");
            pool.AddTokens(2);

            // Put a duplicate on top of deck
            PutOnDeck("Confidence");

            QuickHPStorage(tattletale);
            UsePower(tattletale, 0);

            // Deals X=2 psychic damage to self and clears tokens
            QuickHPCheck(-2);
            Assert.That(pool.CurrentValue, Is.EqualTo(0));
        }

        [Test()]
        public void TestHunterPower1DealsXPsychicAndRemovesToken()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale/TattletaleHunterOfSecretsCharacter", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            // Add 3 tokens
            var pool = tattletale.CharacterCard.FindTokenPool("TattletaleHunterOfSecretsPool");
            pool.AddTokens(3);

            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            UsePower(tattletale, 1);

            // Deals X=3 psychic damage
            QuickHPCheck(-3);
            // Removes 1 token
            Assert.That(pool.CurrentValue, Is.EqualTo(2));
        }

        [Test()]
        public void TestHunterPower1NoTokensDoesNothing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale/TattletaleHunterOfSecretsCharacter", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var pool = tattletale.CharacterCard.FindTokenPool("TattletaleHunterOfSecretsPool");
            Assert.That(pool.CurrentValue, Is.EqualTo(0));

            QuickHPStorage(baron);
            UsePower(tattletale, 1);

            // No tokens, nothing happens
            QuickHPCheck(0);
        }

        #endregion
    }
}
