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
    public class UnrelentingStingsTests : BaseTest
    {
        protected HeroTurnTakerController skitter { get { return FindHero("Skitter"); } }
        protected HeroTurnTakerController tattletale { get { return FindHero("Tattletale"); } }

        [Test()]
        public void CanDealNoDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            GoToDrawCardPhase(skitter);

            PlayCard("UnrelentingStings");

            QuickHPStorage(baron, skitter);
            DecisionSelectTargets = new Card[] { null };

            GoToEndOfTurn(skitter);
            QuickHPCheck(0, 0);
        }

        [Test()]
        public void NoTokens1Target()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            GoToDrawCardPhase(skitter);

            var battalion = PlayCard("BladeBattalion");

            PlayCard("UnrelentingStings");

            QuickHPStorage(baron.CharacterCard, skitter.CharacterCard, battalion);
            DecisionSelectTargets = new Card[] { baron.CharacterCard, battalion };

            GoToEndOfTurn(skitter);
            QuickHPCheck(-1, 0, 0);
        }

        [Test()]
        public void OneToken2Targets()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            GoToDrawCardPhase(skitter);

            var battalion = PlayCard("BladeBattalion");

            var bugs = PlayCard("UnrelentingStings");
            var pool = bugs.FindBugPool();
            pool.AddTokens(1);

            QuickHPStorage(baron.CharacterCard, skitter.CharacterCard, battalion);
            DecisionSelectTargets = new Card[] { baron.CharacterCard, skitter.CharacterCard, battalion };

            GoToEndOfTurn(skitter);
            QuickHPCheck(-1, -1, 0);
        }

        [Test()]
        public void TwoTokens3Targets()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            GoToDrawCardPhase(skitter);

            var battalion = PlayCard("BladeBattalion");

            var bugs = PlayCard("UnrelentingStings");
            var pool = bugs.FindBugPool();
            pool.AddTokens(2);

            QuickHPStorage(baron.CharacterCard, skitter.CharacterCard, battalion);
            DecisionSelectTargets = new Card[] { baron.CharacterCard, skitter.CharacterCard, battalion };

            GoToEndOfTurn(skitter);
            QuickHPCheck(-1, -1, -1);
        }
    }
}
