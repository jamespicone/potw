using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Alexandria
{
    [TestFixture()]
    public class ProstheticEyeTests : ParahumanTest
    {
        [Test()]
        public void TestWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            MoveAllCardsFromHandToDeck(alexandria);

            PlayCard("ProstheticEye");

            StackDeck("HastenDoom", "LivingForceField", "PoweredRemoteTurret");
            var topCards = baron.TurnTaker.Deck.GetTopCards(3);

            GoToDrawCardPhase(alexandria);
            AssertNextRevealReveals(topCards.ToList());

            DecisionSelectLocation = new LocationChoice(baron.TurnTaker.Deck);
            GoToEndOfTurn(alexandria);
        }

        [Test()]
        public void TestPutsCardsBackInChosenOrder()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            PlayCard("ProstheticEye");

            StackDeck(baron, new[] { "BacklashField", "ElementalRedistributor", "PoweredRemoteTurret" });
            var topCards = baron.TurnTaker.Deck.GetTopCards(3).ToList();

            // Put the three cards back in reverse order. Selection order
            // becomes the final top-to-bottom order.
            var reversed = new[] { topCards[2], topCards[1], topCards[0] };
            DecisionSelectLocation = new LocationChoice(baron.TurnTaker.Deck);
            DecisionSelectCards = reversed;

            GoToEndOfTurn(alexandria);

            Assert.That(baron.TurnTaker.Deck.GetTopCards(3).ToList(), Is.EqualTo(reversed));
        }
    }
}
