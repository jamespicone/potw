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
    public class StormOfStingersTests : ParahumanTest
    {
        [Test()]
        public void Hits1Target()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            var battalion = PlayCard("BladeBattalion");

            QuickHPStorage(baron.CharacterCard, battalion);
            DecisionSelectCards = new Card[] { baron.CharacterCard, battalion };
            PlayCard("StormOfStingers");
            QuickHPCheck(-4, 0);
        }

        [Test()]
        public void TwoDamageInstances()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            PlayCard("LivingForceField");

            QuickHPStorage(baron.CharacterCard);
            DecisionSelectCards = new Card[] { baron.CharacterCard };
            PlayCard("StormOfStingers");
            QuickHPCheck(-2);
        }

        [Test()]
        public void SpendATokenToHit2Cards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            var battalion = PlayCard("BladeBattalion");

            var bugpool = skitter.CharacterCard.FindBugPool();
            AssertTokenPoolCount(bugpool, 0);

            bugpool.AddTokens(1);

            QuickHPStorage(baron.CharacterCard, battalion);
            DecisionSelectNumber = 1;
            DecisionSelectCards = new Card[] { baron.CharacterCard, battalion };
            PlayCard("StormOfStingers");
            QuickHPCheck(-4, -4);

            AssertTokenPoolCount(bugpool, 0);
        }

        [Test()]
        public void GetATokenMove()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            StartGame();

            var bugpool = skitter.CharacterCard.FindBugPool();
            AssertTokenPoolCount(bugpool, 0);

            bugpool.AddTokens(2);

            MoveAllCards(skitter, skitter.TurnTaker.Deck, skitter.HeroTurnTaker.Hand);
            var ongoings = FindCardsWhere(c => c.Location == skitter.HeroTurnTaker.Hand && c.IsOngoing);

            PlayCards(ongoings);

            var strategies = FindCardsWhere(c => c.Location == skitter.TurnTaker.PlayArea && c.DoKeywordsContain("strategy"));
            var nonStrategies = FindCardsWhere(c => c.Location == skitter.TurnTaker.PlayArea && !c.DoKeywordsContain("strategy"));

            var cardBugPool = strategies.FirstOrDefault().FindBugPool();
            AssertTokenPoolCount(cardBugPool, 0);

            DecisionSelectCards = new List<Card> { skitter.CharacterCard, strategies.FirstOrDefault(), null };

            PlayCard("StormOfStingers");

            AssertTokenPoolCount(bugpool, 1);
            AssertTokenPoolCount(cardBugPool, 1);
        }
    }
}
