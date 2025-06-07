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
    public class FindTests : ParahumanTest
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
            var stackedCards = StackDeck("BladeBattalion", "BacklashField", "MobileDefensePlatform", "ElementalRedistributor");

            // Set up expected revealed cards before playing Find
            AssertNextRevealReveals(stackedCards.ToList());

            DecisionSelectLocation = new LocationChoice(baron.TurnTaker.Deck); // Select Baron's deck

            // Set up decisions for the three targets (Battalion, Platform, Redistributor)
            DecisionSelectFunctions = new int?[] { 1, 0, 1 }; // Play Battalion, discard Platform, play Redistributor

            PlayCard("Find");

            // Verify correct card locations:
            // - BladeBattalion should be in play (decision 1)
            AssertIsInPlay(stackedCards.ElementAt(0));
            // - BacklashField should be in deck (not a target)
            AssertInDeck(stackedCards.ElementAt(1));
            // - MobileDefensePlatform should be in trash (decision 0)
            AssertInTrash(stackedCards.ElementAt(2));
            // - ElementalRedistributor should be in play (decision 1)
            AssertIsInPlay(stackedCards.ElementAt(3));

            // Verify no cards are left in the revealed area
            AssertNumberOfCardsAtLocation(baron.TurnTaker.Revealed, 0);
        }

        [Test()]
        public void TestCanPlayTarget()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "Legacy", "InsulaPrimalis");
            StartGame();

            MoveAllCards(baron, baron.TurnTaker.Deck, baron.TurnTaker.OutOfGame);

            // Stack Baron's deck with a target and non-target
            StackDeck("BladeBattalion", "BacklashField");
            DecisionSelectLocation = new LocationChoice(baron.TurnTaker.Deck); // Select Baron's deck

            // Choose to put Battalion into play
            DecisionSelectFunction = 1; // "Put into play" option

            PlayCard("Find");

            // Battalion should be in play, everything else shuffled back
            AssertIsInPlay("BladeBattalion");
            AssertInDeck(GetCard("BacklashField"));
        }

        [Test()]
        public void TestCanDiscardTarget()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "Legacy", "InsulaPrimalis");
            StartGame();

            MoveAllCards(baron, baron.TurnTaker.Deck, baron.TurnTaker.OutOfGame);

            // Stack Baron's deck with a target and non-target
            var cards = StackDeck("BladeBattalion", "BacklashField");
            DecisionSelectLocation = new LocationChoice(baron.TurnTaker.Deck); // Select Baron's deck

            // Choose to discard Battalion
            DecisionSelectFunction = 0; // "Discard" option

            PlayCard("Find");

            // Battalion should be in trash, everything else shuffled back
            AssertInTrash(cards.ElementAt(0));
            AssertInDeck(cards.ElementAt(1));
        }

        [Test()]
        public void TestCanHandleMultipleTargets()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "Legacy", "InsulaPrimalis");
            StartGame();

            PlayCard("Brutus"); // Add a dog to get more reveals

            MoveAllCards(baron, baron.TurnTaker.Deck, baron.TurnTaker.OutOfGame);

            // Stack Baron's deck with multiple targets and other cards
            // Cards are stacked bottom-to-top, so BacklashField will be on bottom, Battalion on top
            var cards = StackDeckHandleDuplicates("BacklashField", "PoweredRemoteTurret", "BladeBattalion");
            DecisionSelectLocation = new LocationChoice(baron.TurnTaker.Deck); // Select Baron's deck

            // Play first target (Battalion), discard second one (Turret)
            DecisionSelectFunctions = new int?[] { 1, 0 }; // Play Battalion, discard Turret

            PlayCard("Find");

            // Should have one in play, one in trash
            AssertIsInPlay(cards.ElementAt(2));        // BladeBattalion - top card
            AssertInTrash(cards.ElementAt(1));         // PoweredRemoteTurret - middle card
            AssertInDeck(cards.ElementAt(0));          // BacklashField - bottom card
        }

        [Test()]
        public void TestIgnoresNonTargets()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "Legacy", "InsulaPrimalis");
            StartGame();

            MoveAllCards(baron, baron.TurnTaker.Deck, baron.TurnTaker.OutOfGame);

            PlayCard("Milk"); // Add a dog to get more reveals

            // Stack deck with all non-targets
            var cards = StackDeckHandleDuplicates("BacklashField", "LivingForceField", "HastenDoom");
            DecisionSelectLocation = new LocationChoice(baron.TurnTaker.Deck);

            PlayCard("Find");

            // All cards should be shuffled back since none are targets
            AssertNumberOfCardsInDeck(baron, 3);
            AssertAtLocation(cards, baron.TurnTaker.Deck);
        }

        [Test()]
        public void TestCanFindDogs()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "Legacy", "InsulaPrimalis");
            StartGame();

            PlayCard("Milk"); // Add a dog to get more reveals

            // Stack Bitch's deck with dog cards and non-targets
            var cards = StackDeckHandleDuplicates("Brutus", "Judas", "Whistle");
            DecisionSelectLocation = new LocationChoice(bitch.TurnTaker.Deck);

            // Play first dog, discard second one
            DecisionSelectFunctions = new int?[] { 1, 0 }; // Play Brutus, discard Judas

            PlayCard("Find");

            // Should have one dog in play, one in trash
            AssertIsInPlay(cards.ElementAt(1));
            AssertInTrash(cards.ElementAt(0));
            AssertInDeck(cards.ElementAt(2)); // Whistle
        }
    }
}