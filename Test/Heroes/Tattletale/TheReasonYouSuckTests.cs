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
    public class TheReasonYouSuckTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();
        }

        [Test()]
        public void TestIsOneShot()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            var card = GetCard("TheReasonYouSuck");
            Assert.That(card.DoKeywordsContain("one-shot"), Is.True);
        }

        [Test()]
        public void TestDeals4PsychicDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            PlayCard("TheReasonYouSuck");
            QuickHPCheck(-4);
        }
    }
}
