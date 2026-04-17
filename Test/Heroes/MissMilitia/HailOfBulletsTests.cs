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
    public class HailOfBulletsTests : ParahumanTest
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

            var card = GetCard("HailOfBullets");
            Assert.That(card.IsOneShot, Is.True);
        }

        [Test()]
        public void TestDeals2DamageToEachNonHeroTarget()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var bladeBattalion = PlayCard("BladeBattalion");

            QuickHPStorage(baron.CharacterCard, bladeBattalion, missmilitia.CharacterCard, bunker.CharacterCard);
            PlayCard("HailOfBullets");
            // 2 projectile to each non-hero target; heroes are unaffected
            QuickHPCheck(-2, -2, 0, 0);
        }

        [Test()]
        public void TestDoesNotHitHeroes()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            QuickHPStorage(bunker);
            PlayCard("HailOfBullets");
            QuickHPCheck(0);
        }
    }
}
