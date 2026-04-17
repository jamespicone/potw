using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.MissMilitia
{
    [TestFixture()]
    public class CoverFireTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "InsulaPrimalis");
        }

        [Test()]
        public void TestIsOneShot()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "InsulaPrimalis");
            StartGame();

            var card = GetCard("CoverFire");
            Assert.That(card.IsOneShot, Is.True);
        }

        [Test()]
        public void TestDeals2ProjectileDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            DecisionSelectTarget = baron.CharacterCard;
            DecisionSelectCard = missmilitia.CharacterCard;

            QuickHPStorage(baron);
            PlayCard("CoverFire");
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestHeals2HP()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            SetHitPoints(missmilitia, 20);

            DecisionSelectTarget = baron.CharacterCard;
            DecisionSelectCard = missmilitia.CharacterCard;

            QuickHPStorage(missmilitia);
            PlayCard("CoverFire");
            QuickHPCheck(2);
        }
    }
}
