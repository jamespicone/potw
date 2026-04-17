using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Tattletale
{
    [TestFixture()]
    public class MyEyeOnYouTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();
        }

        [Test()]
        public void TestIsOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            var card = GetCard("MyEyeOnYou");
            Assert.That(card.DoKeywordsContain("ongoing"), Is.True);
        }

        [Test()]
        public void TestPowerHealsHeroTarget3HP()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            SetHitPoints(bunker, 20);

            var card = PlayCard("MyEyeOnYou");
            DecisionSelectCard = bunker.CharacterCard;

            QuickHPStorage(bunker);
            UsePower(card);
            QuickHPCheck(3);
        }
    }
}
