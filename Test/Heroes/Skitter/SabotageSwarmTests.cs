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
    public class SabotageSwarmTests : ParahumanTest
    {
        [Test()]
        public void ZeroTokensDestroyOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            var swarm = PlayCard("SabotageSwarm");
            var forcefield = PlayCard("LivingForceField");

            AssertDecisionIsOptional(SelectionType.DestroyCard);
            DecisionSelectCard = forcefield;

            GoToEndOfTurn(skitter);

            AssertNotInPlay(forcefield);
            AssertNotInPlay(swarm);
        }

        [Test()]
        public void ZeroTokensDestroyEnv()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            var swarm = PlayCard("SabotageSwarm");
            var police = PlayCard("PoliceBackup");

            AssertDecisionIsOptional(SelectionType.DestroyCard);
            DecisionSelectCard = police;

            GoToEndOfTurn(skitter);

            AssertNotInPlay(police);
            AssertNotInPlay(swarm);
        }

        [Test()]
        public void OneTokenDestroyOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            var swarm = PlayCard("SabotageSwarm");
            var forcefield = PlayCard("LivingForceField");

            var pool = swarm.FindBugPool();
            pool.AddTokens(1);

            AssertDecisionIsOptional(SelectionType.DestroyCard);
            DecisionSelectCard = forcefield;

            GoToEndOfTurn(skitter);

            AssertNotInPlay(forcefield);
            AssertNotInPlay(swarm);
        }

        [Test()]
        public void TwoTokensDestroyOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            var swarm = PlayCard("SabotageSwarm");
            var forcefield = PlayCard("LivingForceField");

            var pool = swarm.FindBugPool();
            pool.AddTokens(2);

            AssertDecisionIsOptional(SelectionType.DestroyCard);
            DecisionSelectCard = forcefield;
            DecisionSelectFunction = 0;

            GoToEndOfTurn(skitter);

            AssertNotInPlay(forcefield);
            AssertIsInPlay(swarm);

            AssertTokenPoolCount(pool, 0);
        }

        [Test()]
        public void TwoTokensDestroyOngoingAndSwarm()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            var swarm = PlayCard("SabotageSwarm");
            var forcefield = PlayCard("LivingForceField");

            var pool = swarm.FindBugPool();
            pool.AddTokens(2);

            AssertDecisionIsOptional(SelectionType.DestroyCard);
            DecisionSelectCard = forcefield;
            DecisionSelectFunction = 1;

            GoToEndOfTurn(skitter);

            AssertNotInPlay(forcefield);
            AssertNotInPlay(swarm);
        }
    }
}
