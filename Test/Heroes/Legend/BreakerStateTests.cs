using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Legend
{
    [TestFixture()]
    public class BreakerStateTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
        }

        [Test()]
        public void TestIsLimited()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
            StartGame();

            var card = GetCard("BreakerState");
            Assert.That(card.DoKeywordsContain("limited"), Is.True);
        }

        [Test()]
        public void TestReducesDamageDealtByLegend()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("BreakerState");

            QuickHPStorage(baron);
            DealDamage(legend, baron, 3, DamageType.Energy);
            // 3 - 1 reduction = 2
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestReducesDamageDealtToLegend()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
            StartGame();

            PlayCard("BreakerState");

            QuickHPStorage(legend);
            DealDamage(baron, legend, 3, DamageType.Melee);
            // 3 - 1 reduction = 2
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestGrantsAdditionalPowerUse()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var splitshot = PlayCard("Splitshot");
            PlayCard("BreakerState");

            GoToUsePowerPhase(legend);

            // Use Legend's innate power
            DecisionSelectTarget = baron.CharacterCard;
            UsePower(legend);

            // Use Splitshot's power (second power use this turn, granted by BreakerState)
            DecisionSelectTargets = new Card[] { baron.CharacterCard, null };
            UsePower(splitshot);
        }

        [Test()]
        public void TestStartOfTurnDestroyToDrawCard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
            StartGame();

            var breakerState = PlayCard("BreakerState");

            // Choose to destroy
            DecisionYesNo = true;

            QuickHandStorage(legend);
            GoToStartOfTurn(legend);

            AssertInTrash(breakerState);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestStartOfTurnDeclineDestroy()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
            StartGame();

            var breakerState = PlayCard("BreakerState");

            // Decline to destroy
            DecisionYesNo = false;

            QuickHandStorage(legend);
            GoToStartOfTurn(legend);

            AssertIsInPlay(breakerState);
            QuickHandCheck(0);
        }
    }
}
