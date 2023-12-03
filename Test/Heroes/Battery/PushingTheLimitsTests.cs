using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Battery
{
    [TestFixture()]
    public class PushingTheLimitsTests : ParahumanTest
    {
        [Test()]
        public void TestUncharged()
        {
            SetupGameController("AkashBhuta", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();

            MoveAllCardsFromHandToDeck(battery);

            QuickHandStorage(battery);

            PlayCard("PushingTheLimits");

            QuickHandCheck(3);
        }

        [Test()]
        public void TestCharged()
        {
            SetupGameController("AkashBhuta", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();

            UsePower(battery);

            MoveAllCards(battery, battery.TurnTaker.Deck, battery.HeroTurnTaker.Hand);

            var staticCharge = GetCardFromHand(battery, "StaticCharge");
            var threads = GetCardFromHand(battery, "GlowingThreads");

            DecisionSelectCards = new Card[] { staticCharge, threads };

            PlayCard("PushingTheLimits");

            AssertIsInPlay(staticCharge);
            AssertIsInPlay(threads);
        }
    }
}
