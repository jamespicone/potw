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
    public class NamedAfterTheLibraryTests : ParahumanTest
    {
        [Test()]
        public void TestShufflesTrash()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            MoveAllCardsFromHandToDeck(alexandria);
            MoveAllCards(alexandria, alexandria.TurnTaker.Deck, alexandria.TurnTaker.Trash, leaveSomeCards: 20);

            StackDeckAfterShuffle(alexandria, new string[] { "Invincible", "Protector", "PureStrength", "TimeLocked" }, source: alexandria.CharacterCardController);
            PlayCard("NamedAfterTheLibrary");

            AssertNumberOfCardsInTrash(alexandria, 0);
            AssertNumberOfCardsInDeck(alexandria, 39);

            var topCardIds = alexandria.TurnTaker.Deck.GetTopCards(4).Select(c => c.Identifier);
            Assert.That(topCardIds, Is.EquivalentTo(new string[] { "Invincible", "Protector", "PureStrength", "TimeLocked" }));
        }

        [Test()]
        public void TestDrawReplacement()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            MoveAllCardsFromHandToDeck(alexandria);

            PlayCard("NamedAfterTheLibrary");

            StackDeck("PureStrength", "TimeLocked", "Protector");

            var topCards = alexandria.HeroTurnTaker.Deck.GetTopCards(3);
            var strength = topCards.Last();
            var protector = topCards.First();
            var timelocked = topCards.ElementAt(1);

            AssertNextRevealReveals(topCards.Take(2).ToList());

            DecisionSelectCard = timelocked;
            DrawCard(alexandria);

            AssertInHand(alexandria, timelocked);
            AssertOnTopOfDeck(protector);

            DecisionSelectCard = protector;

            AssertNextRevealReveals(protector, strength);
            DrawCard(alexandria);

            AssertInHand(alexandria, protector);
            AssertOnTopOfDeck(strength);
        }
    }
}
