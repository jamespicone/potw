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
    public class GloriousStandTests : ParahumanTest
    {
        [Test()]
        public void TestRedirectsHeroDamage()
        {
            // Use Bunker to avoid nemesis bonus
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            PlayCard("GloriousStand");

            QuickHPStorage(dauntless, bunker);
            DealDamage(baron, bunker, 3, DamageType.Melee);

            // Damage redirected to Dauntless
            QuickHPCheck(-3, 0);
        }

        [Test()]
        public void TestRedirectsAllHeroDamage()
        {
            // Use Bunker and Tachyon to avoid nemesis bonus
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "Tachyon", "InsulaPrimalis");
            StartGame();

            PlayCard("GloriousStand");

            QuickHPStorage(dauntless, bunker, tachyon);
            DealDamage(baron, bunker, 2, DamageType.Melee);
            DealDamage(baron, tachyon, 2, DamageType.Melee);

            // All damage redirected to Dauntless
            QuickHPCheck(-4, 0, 0);
        }

        [Test()]
        public void TestDoesNotRedirectVillainDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("GloriousStand");

            QuickHPStorage(baron);
            DealDamage(bunker, baron, 3, DamageType.Melee);

            // Damage to Baron is not redirected
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestDoesNotRedirectEnvironmentDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            PlayCard("GloriousStand");
            var raptor = PlayCard("VelociraptorPack");

            var raptorStartHP = raptor.HitPoints;
            DealDamage(bunker, raptor, 3, DamageType.Melee);

            // Damage to raptor is not redirected
            Assert.That(raptor.HitPoints, Is.EqualTo(raptorStartHP - 3));
        }

        [Test()]
        public void TestDestroysItselfAtStartOfTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            var gloriousStand = PlayCard("GloriousStand");
            AssertIsInPlay(gloriousStand);

            GoToStartOfTurn(dauntless);

            AssertInTrash(gloriousStand);
        }

        [Test()]
        public void TestRedirectsUntilStartOfTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            GoToPlayCardPhase(dauntless);

            PlayCard("GloriousStand");

            // Damage during Dauntless's turn
            QuickHPStorage(dauntless, bunker);
            DealDamage(baron, bunker, 3, DamageType.Melee);
            QuickHPCheck(-3, 0); // Redirected

            GoToPlayCardPhase(bunker);

            // Damage during Bunker's turn (before Dauntless's next start of turn)
            QuickHPStorage(dauntless, bunker);
            DealDamage(baron, bunker, 3, DamageType.Melee);
            QuickHPCheck(-3, 0); // Still redirected
        }

        [Test()]
        public void TestRedirectsOwnDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            PlayCard("GloriousStand");

            // Dauntless deals damage to Bunker - should redirect to self
            QuickHPStorage(dauntless, bunker);
            DealDamage(dauntless, bunker, 3, DamageType.Melee);

            QuickHPCheck(-3, 0); // Redirected to Dauntless
        }

        [Test()]
        public void TestDamageToDauntlessIsNotRedirected()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            PlayCard("GloriousStand");

            // Direct damage to Dauntless
            QuickHPStorage(dauntless);
            DealDamage(baron, dauntless, 3, DamageType.Melee);

            QuickHPCheck(-3); // Direct damage, no infinite loop
        }

        [Test()]
        public void TestNotLimited()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            var stand1 = GetCard("GloriousStand", 0);
            var stand2 = GetCard("GloriousStand", 1);

            PlayCard(stand1);
            PlayCard(stand2);

            // Both should be in play (not limited)
            AssertIsInPlay(stand1);
            AssertIsInPlay(stand2);
        }

        [Test()]
        public void TestRedirectsHeroTargets()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            // Play a hero equipment target
            var turretMode = PlayCard("TurretMode");

            PlayCard("GloriousStand");

            QuickHPStorage(dauntless, bunker);

            // Damage to Bunker should be redirected to Dauntless
            DealDamage(baron, bunker, 3, DamageType.Melee);
            QuickHPCheck(-3, 0); // 3 damage redirected to Dauntless
        }
    }
}
