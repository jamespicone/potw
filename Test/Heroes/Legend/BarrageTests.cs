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
    public class BarrageTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
        }

        [Test()]
        public void TestIsOneShot()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
            StartGame();

            var card = GetCard("Barrage");
            Assert.That(card.IsOneShot, Is.True);
        }

        [Test()]
        public void TestDeals3EnergyDamageToAllVillainTargets()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var bladeBattalion = PlayCard("BladeBattalion");

            QuickHPStorage(baron.CharacterCard, bladeBattalion, legend.CharacterCard, bunker.CharacterCard);

            AssertDamageSource(legend.CharacterCard);
            AssertDamageType(DamageType.Energy);

            PlayCard("Barrage");

            // Baron and Battalion both take 3, heroes take 0
            QuickHPCheck(-3, -3, 0, 0);
        }

        [Test()]
        public void TestDoesNotHitHeroTargets()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            QuickHPStorage(legend.CharacterCard, bunker.CharacterCard);

            PlayCard("Barrage");

            QuickHPCheck(0, 0);
        }

        [Test()]
        public void TestDoesNotHitEnvironmentTargets()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
            StartGame();

            var envTarget = PlayCard("VelociraptorPack");

            QuickHPStorage(envTarget);
            PlayCard("Barrage");
            QuickHPCheck(0);
        }
    }
}
