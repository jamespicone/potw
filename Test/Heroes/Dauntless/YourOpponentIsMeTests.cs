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
    public class YourOpponentIsMeTests : ParahumanTest
    {
        [Test()]
        public void TestDealsDamageToTwoTargets()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            // Remove MDP (it makes Baron immune) and add another villain target
            RemoveMobileDefensePlatform();
            var battalion = PlayCard("BladeBattalion");

            QuickHPStorage(baron);
            DecisionSelectTargets = new Card[] { baron.CharacterCard, battalion };
            AssertDamageSource(dauntless.CharacterCard, dauntless.CharacterCard);
            AssertDamageType(DamageType.Energy, DamageType.Energy);

            PlayCard("YourOpponentIsMe");

            QuickHPCheck(-2);
            Assert.That(battalion.HitPoints, Is.EqualTo(battalion.MaximumHitPoints - 2));
        }

        [Test()]
        public void TestCanTargetOneTarget()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            QuickHPStorage(baron);
            DecisionSelectTargets = new Card[] { baron.CharacterCard, null };

            PlayCard("YourOpponentIsMe");

            QuickHPCheck(-2);
        }

        [Test()]
        public void TestCanTargetZeroTargets()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            QuickHPStorage(baron);
            DecisionDoNotSelectCard = SelectionType.SelectTarget;

            PlayCard("YourOpponentIsMe");

            QuickHPCheck(0); // No damage
        }

        [Test()]
        public void TestRedirectsHitTargetsDamage()
        {
            // Use Bunker instead of Legacy to avoid nemesis damage bonus
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            DecisionSelectTargets = new Card[] { baron.CharacterCard, null };

            PlayCard("YourOpponentIsMe");

            // Baron's damage should now redirect to Dauntless
            QuickHPStorage(dauntless, bunker);
            DealDamage(baron, bunker, 3, DamageType.Melee);

            // Damage redirected to Dauntless
            QuickHPCheck(-3, 0);
        }

        [Test()]
        public void TestOnlyRedirectsHitTargets()
        {
            // Use Bunker instead of Legacy to avoid nemesis damage bonus
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            // Remove MDP first (it makes Baron immune) and play a new one
            RemoveMobileDefensePlatform();
            var mdp = PlayCard("MobileDefensePlatform");

            // Only hit mdp, not Baron
            DecisionSelectTargets = new Card[] { mdp, null };

            PlayCard("YourOpponentIsMe");

            // MDP's damage redirects
            QuickHPStorage(dauntless, bunker);
            DealDamage(mdp, bunker, 3, DamageType.Melee);
            QuickHPCheck(-3, 0);

            // Baron's damage does NOT redirect (wasn't hit)
            QuickHPStorage(dauntless, bunker);
            DealDamage(baron, bunker, 3, DamageType.Melee);
            QuickHPCheck(0, -3);
        }

        [Test()]
        public void TestRedirectExpiresAtStartOfNextTurn()
        {
            // Use Bunker instead of Legacy to avoid nemesis damage bonus
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            DecisionSelectTargets = new Card[] { baron.CharacterCard, null };

            PlayCard("YourOpponentIsMe");

            // Go to start of Dauntless's next turn
            GoToStartOfTurn(dauntless);

            // Redirect should be expired
            QuickHPStorage(dauntless, bunker);
            DealDamage(baron, bunker, 3, DamageType.Melee);

            // Damage goes to Bunker (no longer redirected)
            QuickHPCheck(0, -3);
        }

        [Test()]
        public void TestRedirectBothTargets()
        {
            // Use Bunker instead of Legacy to avoid nemesis damage bonus
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            // Remove MDP (it makes Baron immune)
            RemoveMobileDefensePlatform();

            // Add another villain target that doesn't have immunity interactions
            var battalion = PlayCard("BladeBattalion");

            // Hit both Baron and Battalion
            DecisionSelectTargets = new Card[] { baron.CharacterCard, battalion };

            PlayCard("YourOpponentIsMe");

            // Both targets' damage should redirect
            QuickHPStorage(dauntless, bunker);
            DealDamage(baron, bunker, 2, DamageType.Melee);
            DealDamage(battalion, bunker, 2, DamageType.Melee);

            QuickHPCheck(-4, 0);
        }

        [Test()]
        public void TestDamageSourceIsDauntless()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            AssertDamageSource(dauntless.CharacterCard);
            DecisionSelectTargets = new Card[] { baron.CharacterCard, null };

            PlayCard("YourOpponentIsMe");
        }

        [Test()]
        public void TestDamageTypeIsEnergy()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            AssertDamageType(DamageType.Energy);
            DecisionSelectTargets = new Card[] { baron.CharacterCard, null };

            PlayCard("YourOpponentIsMe");
        }

        [Test()]
        public void TestNoRedirectIfNoTargetSelected()
        {
            // Verify that if we don't hit any targets, there's no redirect
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            // Skip target selection entirely
            DecisionDoNotSelectCard = SelectionType.SelectTarget;

            PlayCard("YourOpponentIsMe");

            // Baron's damage should NOT redirect (no targets were hit)
            QuickHPStorage(dauntless, bunker);
            DealDamage(baron, bunker, 3, DamageType.Melee);

            // Damage goes to Bunker (no redirect set up)
            QuickHPCheck(0, -3);
        }
    }
}
