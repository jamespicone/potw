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
    public class AlwaysPlanningTests : BaseTest
    {
        protected HeroTurnTakerController skitter { get { return FindHero("Skitter"); } }
        protected HeroTurnTakerController tattletale { get { return FindHero("Tattletale"); } }

        [Test()]
        public void TestGainABugOnEnvTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();

            var bugpool = skitter.CharacterCard.FindBugPool();
            AssertTokenPoolCount(bugpool, 0);

            PlayCard("AlwaysPlanning");

            GoToEndOfTurn(skitter);
            AssertTokenPoolCount(bugpool, 0);

            EnterNextTurnPhase();
            AssertTokenPoolCount(bugpool, 1);
        }

        [Test()]
        public void TestPower()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();

            var deck = StackDeck("ImpeccableAim", "VindictiveCreativity", "SwarmOfFlies", "BugClones");
            var expectedPlay = deck.Skip(3).First();
            var alwaysPlanning = PlayCard("AlwaysPlanning");

            AssertNoDecision();
            UsePower(alwaysPlanning);

            AssertIsInPlay(expectedPlay);
            AssertNotInPlay(deck.Take(3));            
        }
    }
}
