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
    public class OutfittingTests : ParahumanTest
    {
        [Test()]
        public void TestSearchOwnDeckForRelic()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var arcshield = GetCard("Arcshield");
            MoveCard(dauntless, arcshield, dauntless.TurnTaker.Deck);

            DecisionSelectTurnTaker = dauntless.TurnTaker;
            DecisionSelectLocation = new LocationChoice(dauntless.TurnTaker.PlayArea);

            PlayCard("Outfitting");

            // Arcshield should be in play
            AssertIsInPlay(arcshield);
        }

        [Test()]
        public void TestSearchOtherHeroDeck()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            var flakCannon = GetCard("FlakCannon");
            MoveCard(bunker, flakCannon, bunker.TurnTaker.Deck);

            DecisionSelectTurnTaker = bunker.TurnTaker;
            DecisionSelectLocation = new LocationChoice(bunker.TurnTaker.PlayArea);

            PlayCard("Outfitting");

            // FlakCannon should be in play
            AssertIsInPlay(flakCannon);
        }

        [Test()]
        public void TestCanPutIntoHand()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            var flakCannon = GetCard("FlakCannon");
            MoveCard(bunker, flakCannon, bunker.TurnTaker.Deck);

            DecisionSelectTurnTaker = bunker.TurnTaker;
            DecisionMoveCardDestination = new MoveCardDestination(bunker.HeroTurnTaker.Hand);

            PlayCard("Outfitting");

            // FlakCannon should be in hand
            AssertInHand(flakCannon);
        }

        [Test()]
        public void TestFindsEquipment()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            // Stack deck with non-equipment cards then equipment
            // OmniCannon is non-equipment (Power), FlakCannon is equipment
            var flakCannon = GetCard("FlakCannon");
            MoveCard(bunker, flakCannon, bunker.TurnTaker.Deck);

            DecisionSelectTurnTaker = bunker.TurnTaker;
            DecisionSelectLocation = new LocationChoice(bunker.TurnTaker.PlayArea);

            PlayCard("Outfitting");

            // FlakCannon should be in play
            AssertIsInPlay(flakCannon);
        }

        [Test()]
        public void TestFindsRelic()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            // Stack deck with non-relic cards then relic
            StackDeck(dauntless, new string[] { "CracklingCorona", "Arcshield" });

            DecisionSelectTurnTaker = dauntless.TurnTaker;
            DecisionSelectLocation = new LocationChoice(dauntless.TurnTaker.PlayArea);

            PlayCard("Outfitting");

            // Arcshield should be in play
            var arcshield = GetCard("Arcshield");
            AssertIsInPlay(arcshield);
        }

        [Test()]
        public void TestShufflesRemainingCards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            // Make sure FlakCannon is somewhere in the deck
            var flakCannon = GetCard("FlakCannon");
            MoveCard(bunker, flakCannon, bunker.TurnTaker.Deck);

            // Get the deck card count before
            int initialDeckCount = GetNumberOfCardsInDeck(bunker);

            DecisionSelectTurnTaker = bunker.TurnTaker;
            DecisionSelectLocation = new LocationChoice(bunker.TurnTaker.PlayArea);

            PlayCard("Outfitting");

            // Deck should have one less card (the equipment was put in play)
            Assert.That(GetNumberOfCardsInDeck(bunker), Is.EqualTo(initialDeckCount - 1));
        }

        [Test()]
        public void TestNoEquipmentOrRelicInDeck()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            // Remove all equipment from Bunker's deck
            MoveAllCards(bunker, bunker.TurnTaker.Deck, bunker.TurnTaker.Trash);

            // Add some non-equipment cards back
            var auxiliaryPowerSource = GetCard("AuxiliaryPowerSource");
            MoveCard(bunker, auxiliaryPowerSource, bunker.TurnTaker.Deck);

            DecisionSelectTurnTaker = bunker.TurnTaker;

            // Should still work, just no card found
            PlayCard("Outfitting");
        }
    }
}
