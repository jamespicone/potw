using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Dauntless
{
    [TestFixture()]
    public class CracklingCoronaTests : ParahumanTest
    {
        [Test()]
        public void TestDealsEnergyDamageToAllVillains()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            QuickHPStorage(baron);
            AssertDamageSource(dauntless.CharacterCard);
            AssertDamageType(DamageType.Energy);

            PlayCard("CracklingCorona");

            QuickHPCheck(-1);
        }

        [Test()]
        public void TestDoesNotDamageHeroes()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Legacy", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            QuickHPStorage(dauntless.CharacterCard, legacy.CharacterCard, baron.CharacterCard);

            PlayCard("CracklingCorona");

            // Only Baron takes damage
            QuickHPCheck(0, 0, -1);
        }

        [Test()]
        public void TestDoesNotDamageEnvironment()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var raptor = PlayCard("VelociraptorPack");

            QuickHPStorage(baron.CharacterCard, raptor);

            PlayCard("CracklingCorona");

            // Only Baron takes damage, not raptor
            QuickHPCheck(-1, 0);
        }

        [Test()]
        public void TestDamageSourceIsDauntless()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            AssertDamageSource(dauntless.CharacterCard);
            PlayCard("CracklingCorona");
        }

        [Test()]
        public void TestDamageTypeIsEnergy()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            AssertDamageType(DamageType.Energy);
            PlayCard("CracklingCorona");
        }

        [Test()]
        public void TestHitsMultipleVillainTargets()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            // Put another villain target into play
            PlayCard("BladeBattalion");
            var battalion = FindCardsWhere(c => c.Identifier == "BladeBattalion" && c.IsInPlay).First();

            QuickHPStorage(baron);
            var battalionStartHP = battalion.HitPoints;

            PlayCard("CracklingCorona");

            QuickHPCheck(-1);
            Assert.That(battalion.HitPoints, Is.EqualTo(battalionStartHP - 1));
        }
    }
}
