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
    public class RecyclingTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            // Put some equipment in trash
            PutInTrash("ParahumansOnline");

            DecisionSelectCards = new Card[] { GetCard("ParahumansOnline") };
            var card = PlayCard("Recycling");
            AssertInTrash(card); // One-shot goes to trash
        }

        [Test()]
        public void TestShufflesEquipmentFromTrash()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var equipment = PutInTrash("ParahumansOnline");
            AssertInTrash(equipment);

            DecisionSelectCards = new Card[] { equipment, null };
            PlayCard("Recycling");

            AssertInDeck(equipment);
        }

        [Test()]
        public void TestShufflesDeviceFromTrash()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var device = PutInTrash("ServerFarm");
            AssertInTrash(device);

            DecisionSelectCards = new Card[] { device, null };
            PlayCard("Recycling");

            AssertInDeck(device);
        }

        [Test()]
        public void TestCanSelectUpTo2Cards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var equipment = PutInTrash("ParahumansOnline");
            var device = PutInTrash("ServerFarm");

            DecisionSelectCards = new Card[] { equipment, device };
            PlayCard("Recycling");

            AssertInDeck(equipment);
            AssertInDeck(device);
        }

        [Test()]
        public void TestCanSelectZeroCards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            // Put 2 equipment/device cards in trash so auto-selection doesn't occur
            var equipment1 = PutInTrash("ParahumansOnline");
            var equipment2 = PutInTrash("ServerFarm");

            // Select nothing (skip both selections)
            DecisionSelectCards = new Card[] { null, null };
            PlayCard("Recycling");

            AssertInTrash(equipment1); // Still in trash
            AssertInTrash(equipment2); // Still in trash
        }

        [Test()]
        public void TestIsOneShot()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var card = GetCard("Recycling");

            Assert.That(card.IsOneShot, Is.True, "Recycling should be a One-Shot");
        }

        [Test()]
        public void TestOnlyTargetsEquipmentAndDevice()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            // Put various cards in trash
            var equipment = PutInTrash("ParahumansOnline");
            var ongoing = PutInTrash("Archives");
            var oneShot = PutInTrash("Analysis");

            // Should only be able to select equipment/device
            DecisionSelectCards = new Card[] { equipment, null };
            PlayCard("Recycling");

            AssertInDeck(equipment);
            AssertInTrash(ongoing); // Not selectable
            AssertInTrash(oneShot); // Not selectable
        }
    }
}
