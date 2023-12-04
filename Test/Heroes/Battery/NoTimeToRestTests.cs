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
    public class NoTimeToRestTests : ParahumanTest
    {
        [Test()]
        public void TestDestroyedWithMagnetism()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();

            var magnetism = PlayCard("Magnetism");
            UsePower(battery);

            var platform = GetMobileDefensePlatform().Card;

            PlayCard("NoTimeToRest");

            var charge = GetCard("StaticCharge");
            MoveCard(battery, charge, battery.HeroTurnTaker.Hand);

            DecisionSelectCards = new Card[] { charge };

            QuickHandStorage(battery);
            UsePower(magnetism);
            QuickHandCheck(0); // drew a card, played a card

            AssertIsInPlay(charge);
        }

        [Test()]
        public void TestDestroyedWithDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();

            var platform = GetMobileDefensePlatform().Card;

            PlayCard("NoTimeToRest");

            var charge = GetCard("StaticCharge");
            MoveCard(battery, charge, battery.HeroTurnTaker.Hand);

            DecisionSelectCards = new Card[] { charge };

            QuickHandStorage(battery);
            DealDamage(battery, platform, 30, DamageType.Infernal);
            QuickHandCheck(0); // drew a card, played a card

            AssertIsInPlay(charge);
        }
    }
}
