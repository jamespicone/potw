using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Jp.ParahumansOfTheWormverse.Skitter;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Skitter
{
    [TestFixture()]
    public class SkitterTests : ParahumanTest
    {
        [Test()]
        public void TestGainABug()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            StartGame();

            var bugpool = skitter.CharacterCard.FindBugPool();
            AssertTokenPoolCount(bugpool, 0);
            DiscardAllCards(skitter);
            UsePower(skitter);
            AssertTokenPoolCount(bugpool, 1);
        }

        [Test()]
        public void TestPlayACard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            StartGame();

            var bugpool = skitter.CharacterCard.FindBugPool();
            AssertTokenPoolCount(bugpool, 0);

            MoveAllCards(skitter, skitter.TurnTaker.Deck, skitter.HeroTurnTaker.Hand);
            var strategies = FindCardsWhere(c => c.Location == skitter.HeroTurnTaker.Hand && c.DoKeywordsContain("strategy"));
            var nonStrategies = FindCardsWhere(c => c.Location == skitter.HeroTurnTaker.Hand && ! c.DoKeywordsContain("strategy"));

            AssertNextDecisionChoices(strategies, nonStrategies);
            DecisionSelectCards = new List<Card> { strategies.FirstOrDefault(), null };
            UsePower(skitter);

            AssertIsInPlay(strategies.FirstOrDefault());
            AssertNotInPlay(strategies.Skip(1));
            AssertNotInPlay(nonStrategies);

            AssertTokenPoolCount(bugpool, 0);
        }

        [Test()]
        public void TestMoveTokenFromSkitter()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            StartGame();

            var bugpool = skitter.CharacterCard.FindBugPool();
            AssertTokenPoolCount(bugpool, 0);

            MoveAllCards(skitter, skitter.TurnTaker.Deck, skitter.HeroTurnTaker.Hand);
            var ongoings = FindCardsWhere(c => c.Location == skitter.HeroTurnTaker.Hand && c.IsOngoing);

            PlayCards(ongoings);

            var strategies = FindCardsWhere(c => c.Location == skitter.TurnTaker.PlayArea && c.DoKeywordsContain("strategy"));
            var nonStrategies = FindCardsWhere(c => c.Location == skitter.TurnTaker.PlayArea && !c.DoKeywordsContain("strategy"));

            var cardBugPool = strategies.FirstOrDefault().FindBugPool();
            AssertTokenPoolCount(cardBugPool, 0);

            DecisionSelectCards = new List<Card> { skitter.CharacterCard, strategies.FirstOrDefault() };
            UsePower(skitter);

            AssertTokenPoolCount(bugpool, 0);
            AssertTokenPoolCount(cardBugPool, 1);
        }

        [Test()]
        public void TestNoMoveIfOnlyCharacterCard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            StartGame();

            var bugpool = skitter.CharacterCard.FindBugPool();
            AssertTokenPoolCount(bugpool, 0);

            bugpool.AddTokens(10);
            AssertTokenPoolCount(bugpool, 10);

            AssertNoDecision();
            AssertNextMessageContains("There are no possible bug token moves");

            var didMove = new List<bool>();
            RunCoroutine(skitter.CharacterCardController.MoveBugTokens(false, false, didMove));

            Assert.That(didMove, Has.Exactly(1).Items);
            Assert.That(didMove.FirstOrDefault(), Is.False);
        }

        [Test()]
        public void TestNoMoveIfNoTokens()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            StartGame();

            var bugpool = skitter.CharacterCard.FindBugPool();
            AssertTokenPoolCount(bugpool, 0);

            PlayCards(c => c.DoKeywordsContain("strategy") && c.Owner == skitter.TurnTaker);

            AssertNoDecision();
            AssertNextMessageContains("There are no possible bug token moves");

            var didMove = new List<bool>();
            RunCoroutine(skitter.CharacterCardController.MoveBugTokens(false, false, didMove));

            Assert.That(didMove, Has.Exactly(1).Items);
            Assert.That(didMove.FirstOrDefault(), Is.False);
        }
    }
}
