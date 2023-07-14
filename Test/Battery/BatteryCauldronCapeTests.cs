using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Jp.ParahumansOfTheWormverse.Battery;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Battery
{
    [TestFixture()]
    public class BatteryCauldronCapeTests : ParahumanTest
    {
        [Test()]
        public void TestFaceDownACard()
        {
            SetupGameController(
                new string[] { "BaronBlade", "Jp.ParahumansOfTheWormverse.Battery", "Megalopolis" },
                promoIdentifiers: new Dictionary<string, string> { { "Jp.ParahumansOfTheWormverse.Battery", "BatteryCauldronCapeCharacter" } }
            );

            StartGame();

            MoveAllCardsFromHandToDeck(battery);

            var cards = StackDeck("CoolToys", "Strength", "RapidRecon");

            var coolToys = cards.ElementAt(0);
            var strength = cards.ElementAt(1);
            var rapidRecon = cards.ElementAt(2);

            AssertInDeck(coolToys);
            AssertInDeck(strength);
            AssertInDeck(rapidRecon);

            UsePower(battery, 0);

            AssertInDeck(coolToys);
            AssertInDeck(strength);
            AssertInPlayArea(battery, rapidRecon);
            AssertFlipped(rapidRecon);

            UsePower(battery, 0);

            AssertInDeck(coolToys);
            AssertInPlayArea(battery, strength);
            AssertFlipped(strength);
            AssertInPlayArea(battery, rapidRecon);
            AssertFlipped(rapidRecon);

            UsePower(battery, 0);

            AssertInPlayArea(battery, coolToys);
            AssertFlipped(coolToys);
            AssertInPlayArea(battery, strength);
            AssertFlipped(strength);
            AssertInPlayArea(battery, rapidRecon);
            AssertFlipped(rapidRecon);
        }

        [Test()]
        public void TestPlayACard()
        {
            SetupGameController(
                new string[] { "BaronBlade", "Jp.ParahumansOfTheWormverse.Battery", "Megalopolis" },
                promoIdentifiers: new Dictionary<string, string> { { "Jp.ParahumansOfTheWormverse.Battery", "BatteryCauldronCapeCharacter" } }
            );

            StartGame();

            DestroyNonCharacterVillainCards();
            MoveAllCardsFromHandToDeck(battery);

            StackDeck("Strength");

            UsePower(battery, 0);

            DecisionSelectTarget = baron.CharacterCard;
            QuickHPStorage(baron);
            UsePower(battery, 1);
            QuickHPCheck(-5);            
        }

        [Test()]
        public void TestOnlyLimitedAndNoPlays()
        {
            SetupGameController(
                new string[] { "BaronBlade", "Jp.ParahumansOfTheWormverse.Battery", "Megalopolis" },
                promoIdentifiers: new Dictionary<string, string> { { "Jp.ParahumansOfTheWormverse.Battery", "BatteryCauldronCapeCharacter" } }
            );

            StartGame();

            DestroyNonCharacterVillainCards();
            MoveAllCardsFromHandToDeck(battery);

            var threadsInPlay = PlayCard("GlowingThreads");
            var threadsOnDeck = StackDeck("GlowingThreads");

            UsePower(battery, 0);

            AssertFlipped(threadsOnDeck);
            AssertInPlayArea(battery, threadsOnDeck);
            AssertInPlayArea(battery, threadsInPlay);

            UsePower(battery, 1);

            AssertInPlayArea(battery, threadsInPlay);
            AssertNotFlipped(threadsOnDeck);
            AssertInTrash(threadsOnDeck);
        }

        [Test()]
        public void TestLimitedBecomesUnplayable()
        {
            SetupGameController(
                new string[] { "BaronBlade", "Jp.ParahumansOfTheWormverse.Battery", "Megalopolis" },
                promoIdentifiers: new Dictionary<string, string> { { "Jp.ParahumansOfTheWormverse.Battery", "BatteryCauldronCapeCharacter" } }
            );

            StartGame();

            DestroyNonCharacterVillainCards();
            MoveAllCardsFromHandToDeck(battery);

            var threadsOnDeck = FindCardsWhere(c => c.Identifier == "GlowingThreads");
            StackDeck(battery, threadsOnDeck);
            var firstCard = threadsOnDeck.ElementAt(0);
            var secondCard = threadsOnDeck.ElementAt(1);

            UsePower(battery, 0);
            UsePower(battery, 0);

            AssertFlipped(firstCard);
            AssertFlipped(secondCard);
            AssertInPlayArea(battery, firstCard);
            AssertInPlayArea(battery, secondCard);

            DecisionSelectCards = new Card[] { firstCard, secondCard, null };
            UsePower(battery, 1);

            AssertNotFlipped(firstCard);
            AssertNotFlipped(secondCard);

            AssertIsInPlay(firstCard);
            AssertInTrash(secondCard);
        }

        [Test()]
        public void TestSelectingUnplayableLimited()
        {
            SetupGameController(
                new string[] { "BaronBlade", "Jp.ParahumansOfTheWormverse.Battery", "Megalopolis" },
                promoIdentifiers: new Dictionary<string, string> { { "Jp.ParahumansOfTheWormverse.Battery", "BatteryCauldronCapeCharacter" } }
            );

            StartGame();

            DestroyNonCharacterVillainCards();
            MoveAllCardsFromHandToDeck(battery);

            var threadsInPlay = PlayCard("GlowingThreads");
            var stack = StackDeck("GlowingThreads", "Strength");

            var threadsOnDeck = stack.ElementAt(0);
            var strength = stack.ElementAt(1);

            UsePower(battery, 0);
            UsePower(battery, 0);

            AssertFlipped(threadsOnDeck);
            AssertInPlayArea(battery, threadsOnDeck);
            AssertFlipped(strength);
            AssertInPlayArea(battery, strength);

            DecisionSelectCards = new Card[] { threadsOnDeck, baron.CharacterCard, null };
            QuickHPStorage(baron);
            UsePower(battery, 1);

            AssertInPlayArea(battery, threadsInPlay);
            AssertNotFlipped(threadsOnDeck);
            AssertInTrash(threadsOnDeck);
            QuickHPCheck(-5);
        }
    }
}
