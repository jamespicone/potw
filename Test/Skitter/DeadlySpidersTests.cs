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
    public class DeadlySpidersTests : ParahumanTest
    {
        [Test()]
        public void CanDealNoDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            PlayCard("DeadlySpiders");

            QuickHPStorage(baron, skitter);
            DecisionSelectTargets = new Card[] { null };

            GoToEndOfTurn(skitter);
            QuickHPCheck(0, 0);
        }

        [Test()]
        public void NoTokens1Damage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            PlayCard("DeadlySpiders");

            QuickHPStorage(baron, skitter);
            DecisionSelectTargets = new Card[] { baron.CharacterCard };

            GoToEndOfTurn(skitter);
            QuickHPCheck(-1, 0);
        }

        [Test()]
        public void OneToken2Damage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            var spiders = PlayCard("DeadlySpiders");
            var pool = spiders.FindBugPool();
            pool.AddTokens(1);

            QuickHPStorage(baron, skitter);
            DecisionSelectTargets = new Card[] { skitter.CharacterCard };

            GoToEndOfTurn(skitter);
            QuickHPCheck(0, -2);
        }

        [Test()]
        public void TwoTokens3Damage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            var spiders = PlayCard("DeadlySpiders");
            var pool = spiders.FindBugPool();
            pool.AddTokens(2);

            QuickHPStorage(baron, skitter);
            DecisionSelectTargets = new Card[] { baron.CharacterCard };

            GoToEndOfTurn(skitter);
            QuickHPCheck(-3, 0);
        }

        [Test()]
        public void ThreeTokens4Damage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            var spiders = PlayCard("DeadlySpiders");
            var pool = spiders.FindBugPool();
            pool.AddTokens(3);

            QuickHPStorage(baron, skitter);
            DecisionSelectTargets = new Card[] { skitter.CharacterCard };

            GoToEndOfTurn(skitter);
            QuickHPCheck(0, -4);
        }

        [Test()]
        public void FourTokens4Damage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            var spiders = PlayCard("DeadlySpiders");
            var pool = spiders.FindBugPool();
            pool.AddTokens(4);

            QuickHPStorage(baron, skitter);
            DecisionSelectTargets = new Card[] { baron.CharacterCard };

            GoToEndOfTurn(skitter);
            QuickHPCheck(-4, 0);
        }
    }
}
