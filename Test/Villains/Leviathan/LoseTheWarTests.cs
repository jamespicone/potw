using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Leviathan
{
    [TestFixture()]
    public class LoseTheWarTests : LeviathanTestBase
    {
        private Card SetupLoseTheWar()
        {
            RemoveVillainTriggers();
            return PutTacticInPlay("LoseTheWar");
        }

        [Test()]
        public void TestExilesTopOfEnvironmentDeck()
        {
            SetupLeviathanGame();
            SetupLoseTheWar();

            var pileup = StackDeck("TrafficPileup");

            GoToStartOfTurn(leviathan);

            AssertOutOfGame(pileup);
        }

        [Test()]
        public void TestExilesEnvironmentCardInPlayWhenDeckAndTrashEmpty()
        {
            SetupLeviathanGame();
            SetupLoseTheWar();

            var pileup = PlayCard("TrafficPileup");
            RemoveEnvironmentDeck();

            GoToStartOfTurn(leviathan);

            AssertOutOfGame(pileup);
        }

        [Test()]
        public void TestHeroesLoseWhenNothingToRemove()
        {
            SetupLeviathanGame();
            SetupLoseTheWar();

            RemoveEnvironmentDeck();

            GoToStartOfTurn(leviathan);

            AssertGameOver(EndingResult.AlternateDefeat);
        }

        [Test()]
        public void TestIndestructible()
        {
            SetupLeviathanGame();
            var loseTheWar = SetupLoseTheWar();

            DestroyCard(loseTheWar);

            AssertIsInPlay(loseTheWar);
        }
    }
}
