using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Armsmaster
{
    [TestFixture()]
    public class ReturnInAFlashTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();
        }

        [Test()]
        public void TestIsOneShot()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var card = GetCard("ReturnInAFlash");
            Assert.That(card.DoKeywordsContain("one-shot"), Is.True);
        }

        [Test()]
        public void TestSearchDeckForHalberd()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var halberd = GetCard("OriginalHalberd");
            MoveCard(armsmaster, halberd, armsmaster.TurnTaker.Deck);

            DecisionSelectLocation = new LocationChoice(armsmaster.TurnTaker.Deck);
            DecisionMoveCardDestination = new MoveCardDestination(armsmaster.TurnTaker.PlayArea);

            PlayCard("ReturnInAFlash");

            AssertIsInPlay(halberd);
        }

        [Test()]
        public void TestSearchTrashForHalberd()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var halberd = GetCard("OriginalHalberd");
            MoveCard(armsmaster, halberd, armsmaster.TurnTaker.Trash);

            DecisionSelectLocation = new LocationChoice(armsmaster.TurnTaker.Trash);
            DecisionMoveCardDestination = new MoveCardDestination(armsmaster.TurnTaker.PlayArea);

            PlayCard("ReturnInAFlash");

            AssertIsInPlay(halberd);
        }

        [Test()]
        public void TestOnlyFindsHalberds()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            // Move all halberds out of the deck
            var halberds = FindCardsWhere(c => c.DoKeywordsContain("halberd") && c.Location == armsmaster.TurnTaker.Deck);
            foreach (var h in halberds)
            {
                MoveCard(armsmaster, h, armsmaster.TurnTaker.OutOfGame);
            }

            DecisionSelectLocation = new LocationChoice(armsmaster.TurnTaker.Deck);

            // No halberds in deck, should reveal all cards and find none
            PlayCard("ReturnInAFlash");

            // All halberds still out of game
            foreach (var h in halberds)
            {
                Assert.That(h.Location.Name, Is.Not.EqualTo("Hand"));
                Assert.That(h.IsInPlay, Is.False);
            }
        }
    }
}
