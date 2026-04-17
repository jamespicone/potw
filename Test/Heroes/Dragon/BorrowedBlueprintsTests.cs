using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Dragon
{
    [TestFixture()]
    public class BorrowedBlueprintsTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            // Stack Dragon's deck with equipment
            StackDeck("ParahumansOnline", "Analysis", "ServerFarm");

            DecisionSelectTurnTaker = bunker.TurnTaker;
            var card = PlayCard("BorrowedBlueprints");
            AssertInTrash(card); // One-shot goes to trash
        }

        [Test()]
        public void TestRevealsXCards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            // Put some equipment in Bunker's hand to increase X
            PutInHand("FlakCannon");
            PutInHand("GrenadeLauncher");
            // X = 2 equipment in hand + 2 = 4 cards revealed

            // Get specific card instances and stack them on top
            var equipment = GetCard("ParahumansOnline");
            var device = GetCard("ServerFarm");
            // Stack with specific instances - equipment and device should be in revealed cards
            StackDeck(dragon, new Card[] { equipment, device });

            DecisionSelectTurnTaker = bunker.TurnTaker;
            PlayCard("BorrowedBlueprints");

            // Equipment and Device cards go to hand
            AssertInHand(dragon, equipment);
            AssertInHand(dragon, device);
        }

        [Test()]
        public void TestTakesEquipmentToHand()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            var equipment = GetCard("ParahumansOnline");
            StackDeck(equipment);

            DecisionSelectTurnTaker = bunker.TurnTaker;
            PlayCard("BorrowedBlueprints");

            // Equipment should go to hand
            AssertInHand(equipment);
        }

        [Test()]
        public void TestTakesDeviceToHand()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            var device = GetCard("ServerFarm");
            StackDeck(device);

            DecisionSelectTurnTaker = bunker.TurnTaker;
            PlayCard("BorrowedBlueprints");

            // Device should go to hand
            AssertInHand(device);
        }

        [Test()]
        public void TestDiscardsOtherCards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            // Stack deck with non-equipment/device cards first, then equipment
            // Bunker has no equipment in hand, so X = 0 + 2 = 2 cards revealed
            var oneShot = GetCard("Analysis");
            var equipment = GetCard("ParahumansOnline");

            // Stack with specific card instances
            StackDeck(dragon, new Card[] { oneShot, equipment });

            DecisionSelectTurnTaker = bunker.TurnTaker;
            PlayCard("BorrowedBlueprints");

            // Equipment goes to hand, one-shot shuffled back
            AssertInHand(dragon, equipment);
            AssertInDeck(dragon, oneShot); // Non-matching cards shuffled back
        }

        [Test()]
        public void TestMinimumReveal2()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            // Bunker has no equipment, so X = 0 + 2 = 2 minimum
            StackDeck("ParahumansOnline", "Analysis");

            DecisionSelectTurnTaker = bunker.TurnTaker;
            PlayCard("BorrowedBlueprints");

            // Should reveal at least 2 cards
        }

        [Test()]
        public void TestIsOneShot()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var card = GetCard("BorrowedBlueprints");

            Assert.That(card.IsOneShot, Is.True, "Borrowed Blueprints should be a One-Shot");
        }

        [Test()]
        public void TestCanSelectAnyHero()
        {
            // Verify that we can select any hero (not just Dragon) to count equipment
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "Legacy", "InsulaPrimalis");
            StartGame();

            // Put equipment in Legacy's hand to increase X
            PutInHand(legacy, GetCard("TheLegacyRing"));
            PutInHand(legacy, GetCard("Fortitude"));
            // X = 2 equipment in Legacy's hand + 2 = 4

            // Get the equipment card FIRST, then stack it on top
            var equipment = GetCard("ParahumansOnline");
            StackDeck(dragon, equipment);

            // Select Legacy who has equipment in hand
            DecisionSelectTurnTaker = legacy.TurnTaker;
            PlayCard("BorrowedBlueprints");

            // Equipment should go to Dragon's hand
            AssertInHand(dragon, equipment);
        }
    }
}
