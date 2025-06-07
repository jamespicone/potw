using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Bitch
{
    [TestFixture()]
    public class FetchTests : ParahumanTest
    {
        [Test()]
        public void TestRevealsDogCountPlusTwo()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "Legacy", "InsulaPrimalis");
            StartGame();

            var brutus = PlayCard("Brutus");
            var judas = PlayCard("Judas");
            // Should reveal 4 cards (2 dogs + 2)

            // Stack deck with different cards
            var stackedCards = StackDeck("Angelica", "Bastard", "Bullet", "Ginger");

            // Set up expected revealed cards before playing Fetch
            AssertNextRevealReveals(stackedCards.ToList());

            DecisionSelectLocation = new LocationChoice(bitch.TurnTaker.Deck); // Select Bitch's deck
            PlayCard("Fetch");

            // Verify cards went back to deck
            AssertNumberOfCardsAtLocation(bitch.TurnTaker.Revealed, 0);
            AssertAtLocation(stackedCards, bitch.TurnTaker.Deck);
        }

        [Test()]
        public void TestCanPlayEquipment()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "Legacy", "InsulaPrimalis");
            StartGame();

            // Stack deck with mix of equipment and non-equipment
            StackDeck("Whistle", "Bastard");
            DecisionSelectLocation = new LocationChoice(bitch.TurnTaker.Deck); // Select Bitch's deck

            // Choose to put Whistle into play
            DecisionSelectFunction = 1; // "Put into play" option

            PlayCard("Fetch");

            // Whistle should be in play, everything else shuffled back
            AssertIsInPlay("Whistle");
            AssertInDeck(GetCard("Bastard"));
        }

        [Test()]
        public void TestCanDiscardEquipment()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "Legacy", "InsulaPrimalis");
            StartGame();

            // Stack deck with equipment card
            StackDeck("Whistle", "Bastard");
            DecisionSelectLocation = new LocationChoice(bitch.TurnTaker.Deck); // Select Bitch's deck

            // Choose to discard Whistle
            DecisionSelectFunction = 0; // "Discard" option

            PlayCard("Fetch");

            // Whistle should be in trash
            AssertInTrash("Whistle");
            AssertInDeck(GetCard("Bastard"));
        }

        [Test()]
        public void TestMultipleEquipmentChoice()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "Legacy", "InsulaPrimalis");
            StartGame();

            PlayCard("Milk");

            // Stack deck with multiple equipment
            var cards = StackDeckHandleDuplicates("Whistle", "Whistle", "Bastard");
            DecisionSelectLocation = new LocationChoice(bitch.TurnTaker.Deck); // Select Bitch's deck

            // Play first one, discard second one
            DecisionSelectFunctions = new int?[] { 1, 0 }; // Play first, discard second

            PlayCard("Fetch");

            // Should have one in play, one in trash
            AssertIsInPlay(cards.ElementAt(1));
            AssertInTrash(cards.ElementAt(0));
            AssertNumberOfCardsInTrash(bitch, 2); // Fetch + discarded Whistle
            AssertInDeck(cards.ElementAt(2));
        }

        [Test()]
        public void TestCanPlayDevice()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "Legacy", "InsulaPrimalis");
            StartGame();

            MoveAllCards(baron, baron.TurnTaker.Deck, baron.TurnTaker.OutOfGame);

            PlayCard("Brutus"); // Add a dog to get more reveals

            // Stack Baron's deck with a device and other cards
            StackDeckHandleDuplicates("MobileDefensePlatform", "BladeBattalion", "BladeBattalion");
            DecisionSelectLocation = new LocationChoice(baron.TurnTaker.Deck); // Select Baron's deck

            // Choose to put Platform into play
            DecisionSelectFunction = 1; // "Put into play" option

            PlayCard("Fetch");

            // Platform should be in play, everything else shuffled back
            AssertIsInPlay("MobileDefensePlatform");
            AssertNumberOfCardsInDeck(baron, 2);
        }

        [Test()]
        public void TestCanDiscardDevice()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "Legacy", "InsulaPrimalis");
            StartGame();

            MoveAllCards(baron, baron.TurnTaker.Deck, baron.TurnTaker.OutOfGame);

            PlayCard("Brutus"); // Add a dog to get more reveals

            // Stack Baron's deck with a device and other cards
            StackDeckHandleDuplicates("MobileDefensePlatform", "BladeBattalion", "BladeBattalion");
            DecisionSelectLocation = new LocationChoice(baron.TurnTaker.Deck); // Select Baron's deck

            // Choose to discard Platform
            DecisionSelectFunction = 0; // "Discard" option

            PlayCard("Fetch");

            // Platform should be in trash, everything else shuffled back
            AssertInTrash(GetCard("MobileDefensePlatform"));
            AssertNumberOfCardsInDeck(baron, 2);
        }

        [Test()]
        public void TestCanHandleMultipleDevices()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "Legacy", "InsulaPrimalis");
            StartGame();

            MoveAllCards(baron, baron.TurnTaker.Deck, baron.TurnTaker.OutOfGame);

            PlayCard("Brutus"); // Add a dog to get more reveals

            // Stack Baron's deck with multiple devices and other cards
            var cards = StackDeckHandleDuplicates("MobileDefensePlatform", "ElementalRedistributor", "BladeBattalion");
            DecisionSelectLocation = new LocationChoice(baron.TurnTaker.Deck); // Select Baron's deck

            // Play first device, discard second one
            DecisionSelectFunctions = new int?[] { 1, 0 }; // Play Elemental Redistributor, discard MDP

            PlayCard("Fetch");

            // Should have one in play, one in trash
            AssertIsInPlay(cards.ElementAt(1));
            AssertInTrash(cards.ElementAt(0));
            AssertNumberOfCardsInDeck(baron, 1); // Just BladeBattalion
        }
    }
}