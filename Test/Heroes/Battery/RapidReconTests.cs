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
    public class RapidReconTests : ParahumanTest
    {
        [Test()]
        public void TestCharged()
        {
            SetupGameController("AkashBhuta", "Jp.ParahumansOfTheWormverse.Battery", "Haka", "Legacy", "InsulaPrimalis");

            StartGame();

            UsePower(battery);

            MoveAllCardsFromHandToDeck(battery);
            var staticCharge = GetCard("StaticCharge");
            var recon = GetCard("RapidRecon");

            MoveCard(battery, staticCharge, battery.HeroTurnTaker.Hand);
            MoveCard(battery, recon, battery.HeroTurnTaker.Hand);

            DecisionSelectCard = staticCharge;

            QuickHandStorage(battery, haka, legacy);
            PlayCard(recon);
            // Battery -1 played rapid recon, -1 played static charge, +1 drew a card
            QuickHandCheck(-1, 0, 0);

            AssertIsInPlay(staticCharge);
        }

        [Test()]
        public void TestUncharged()
        {
            SetupGameController("AkashBhuta", "Jp.ParahumansOfTheWormverse.Battery", "Haka", "Legacy", "InsulaPrimalis");

            StartGame();

            MoveAllCardsFromHandToDeck(battery);
            var staticCharge = GetCard("StaticCharge");
            var recon = GetCard("RapidRecon");

            MoveCard(battery, staticCharge, battery.HeroTurnTaker.Hand);
            MoveCard(battery, recon, battery.HeroTurnTaker.Hand);

            DecisionSelectCard = staticCharge;

            QuickHandStorage(battery, haka, legacy);
            PlayCard(recon);
            // Battery -1 played rapid recon, -1 played static charge, +1 drew a card, +1 everyone drew
            QuickHandCheck(0, 1, 1);

            AssertIsInPlay(staticCharge);
        }
    }
}
