using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Grue
{
    [TestFixture()]
    public class LeadershipTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "InsulaPrimalis");

            StartGame();
        }

        [Test()]
        public void TestIncreasesHeroDamageBy1()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("Leadership");

            // Grue's damage is increased
            QuickHPStorage(baron);
            DealDamage(grue, baron, 2, DamageType.Melee);
            QuickHPCheck(-3); // 2 + 1 = 3
        }

        [Test()]
        public void TestAppliesToAllHeroes()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("Leadership");

            // Bunker's damage is also increased
            QuickHPStorage(baron);
            DealDamage(bunker, baron, 2, DamageType.Melee);
            QuickHPCheck(-3); // 2 + 1 = 3
        }

        [Test()]
        public void TestOnlyAppliesToHeroTargets()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("Leadership");

            // Equipment cards are not targets, so their damage is not increased by Leadership
            // Damage from Bunker (a hero target) IS increased
            QuickHPStorage(baron);
            DealDamage(bunker, baron, 2, DamageType.Energy);
            QuickHPCheck(-3); // 2 + 1 (Leadership) = 3

            // Damage from Grue (a hero target) IS increased
            QuickHPStorage(baron);
            DealDamage(grue, baron, 2, DamageType.Melee);
            QuickHPCheck(-3); // 2 + 1 (Leadership) = 3
        }

        [Test()]
        public void TestIsLimited()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var leadership1 = PlayCard("Leadership", 0);
            AssertIsInPlay(leadership1);

            var leadership2 = PlayCard("Leadership", 1);
            // Second copy goes to trash, first stays in play
            AssertIsInPlay(leadership1);
            AssertInTrash(leadership2);
        }

        [Test()]
        public void TestDoesNotAffectVillainDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("Leadership");

            // Baron's damage is NOT increased
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 2, DamageType.Melee);
            QuickHPCheck(-2); // Just 2, not increased
        }

        [Test()]
        public void TestIsOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var leadership = GetCard("Leadership");

            Assert.That(leadership.DoKeywordsContain("ongoing"), Is.True, "Leadership should be ongoing");
        }
    }
}
