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
    public class RegroupTests : ParahumanTest
    {
        [Test()]
        public void AddsAToken()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();
            MoveAllCardsFromHandToDeck(skitter);

            PlayCard("Regroup");

            var pool = skitter.CharacterCard.FindBugPool();
            AssertTokenPoolCount(pool, 1);
        }

        [Test()]
        public void AllowsAPlay()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();
            MoveAllCardsFromHandToDeck(skitter);
            var flies = MoveCard(skitter, "SwarmOfFlies", skitter.HeroTurnTaker.Hand);

            DecisionSelectCards = new Card[] { flies };
            PlayCard("Regroup");

            var pool = skitter.CharacterCard.FindBugPool();
            AssertTokenPoolCount(pool, 3);

            AssertIsInPlay(flies);
        }

        [Test()]
        public void AllowsMoves()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();
            MoveAllCardsFromHandToDeck(skitter);

            var strategies = FindCardsWhere(c => c.Owner == skitter.TurnTaker && c.DoKeywordsContain("strategy"));

            PlayCards(strategies);

            var moves = new List<Card>() { skitter.CharacterCard };
            foreach (var card in strategies)
            {
                moves.Add(card);
                moves.Add(card);
            }
            moves.Add(null);

            DecisionSelectCards = moves;
            PlayCard("Regroup");

            var pool = strategies.Last().FindBugPool();
            AssertTokenPoolCount(pool, 1);
        }
    }
}
