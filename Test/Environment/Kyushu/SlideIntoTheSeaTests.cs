using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Environment.Kyushu
{
    [TestFixture()]
    public class SlideIntoTheSeaTests : KyushuTestBase
    {
        [Test()]
        public void TestEndOfTurnPlaysAndRemovesCards()
        {
            SetupKyushuGame();

            PlayCard("SlideIntoTheSea");

            // Make the end-of-turn play deterministic.
            var sentai = StackDeck("SentaiElite");

            GoToEndOfTurn(env);

            // The top card was played, and a card was removed from the game.
            AssertIsInPlay(sentai);
            Assert.That(env.TurnTaker.OutOfGame.NumberOfCards, Is.EqualTo(1),
                "A card should have been removed from the game");
        }

        [Test()]
        public void TestDestroysAnotherEnvironmentCardInstead()
        {
            SetupKyushuGame();

            var slide = PlayCard("SlideIntoTheSea");
            var sentai = PlayCard("SentaiElite", 0);

            DestroyCard(slide);

            // Slide survives; the other environment card is destroyed instead.
            AssertIsInPlay(slide);
            AssertInTrash(sentai);
        }

        [Test()]
        public void TestGameOverWhenEnvironmentDeckEmpty()
        {
            SetupKyushuGame();

            PlayCard("SlideIntoTheSea");

            MoveCards(env, env.TurnTaker.Deck.Cards.ToList(), env.TurnTaker.OutOfGame);

            GoToStartOfTurn(env);

            AssertGameOver(EndingResult.EnvironmentDefeat);
        }
    }
}
