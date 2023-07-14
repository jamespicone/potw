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
    public class BuryInBugsTests : ParahumanTest
    {
        [Test()]
        public void Destroys1Card()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();

            var backlash = PlayCard("BacklashField");
            var forcefield = PlayCard("LivingForceField");

            DecisionSelectCards = new Card[] { backlash, forcefield };
            PlayCard("BuryInBugs");

            AssertNotInPlay(backlash);
            AssertIsInPlay(forcefield);
        }

        [Test()]
        public void SpendATokenToDestroy2Cards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();

            var bugpool = skitter.CharacterCard.FindBugPool();
            AssertTokenPoolCount(bugpool, 0);

            bugpool.AddTokens(1);

            var backlash = PlayCard("BacklashField");
            var forcefield = PlayCard("LivingForceField");

            DecisionSelectNumber = 1;
            DecisionSelectCards = new Card[] { backlash, forcefield };
            PlayCard("BuryInBugs");

            AssertNotInPlay(backlash);
            AssertNotInPlay(forcefield);

            AssertTokenPoolCount(bugpool, 0);
        }

        [Test()]
        public void Spend2TokensToDestroy3Cards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();

            var bugpool = skitter.CharacterCard.FindBugPool();
            AssertTokenPoolCount(bugpool, 0);

            bugpool.AddTokens(10);

            var backlash = PlayCard("BacklashField");
            var backlash2 = PlayCard("BacklashField");
            var forcefield = PlayCard("LivingForceField");

            DecisionSelectNumber = 2;
            DecisionSelectCards = new Card[] { backlash, forcefield, backlash2 };
            PlayCard("BuryInBugs");

            AssertNotInPlay(backlash);
            AssertNotInPlay(forcefield);
            AssertNotInPlay(backlash2);

            AssertTokenPoolCount(bugpool, 8);
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

            PlayCard("BuryInBugs");

            AssertTokenPoolCount(bugpool, 1);
            AssertTokenPoolCount(cardBugPool, 1);
        }
    }
}
