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
    public class BounceBackTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
        }

        [Test()]
        public void TestIsLimited()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
            StartGame();

            var card = GetCard("BounceBack");
            Assert.That(card.DoKeywordsContain("limited"), Is.True);
        }

        [Test()]
        public void TestGainsChargeTokenWhenLegendTakesDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
            StartGame();

            var bounceBack = PlayCard("BounceBack");
            var pool = bounceBack.FindTokenPool("ChargePool");

            Assert.That(pool.CurrentValue, Is.EqualTo(0));

            DealDamage(baron, legend, 2, DamageType.Melee);

            Assert.That(pool.CurrentValue, Is.EqualTo(1));
        }

        [Test()]
        public void TestGainsMultipleTokensFromMultipleHits()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
            StartGame();

            var bounceBack = PlayCard("BounceBack");
            var pool = bounceBack.FindTokenPool("ChargePool");

            DealDamage(baron, legend, 2, DamageType.Melee);
            DealDamage(baron, legend, 2, DamageType.Melee);
            DealDamage(baron, legend, 2, DamageType.Melee);

            Assert.That(pool.CurrentValue, Is.EqualTo(3));
        }

        [Test()]
        public void TestSpendTokenToIncreaseDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var bounceBack = PlayCard("BounceBack");
            var pool = bounceBack.FindTokenPool("ChargePool");

            // Give Legend a charge token
            DealDamage(baron, legend, 2, DamageType.Melee);
            Assert.That(pool.CurrentValue, Is.EqualTo(1));

            // Choose to spend token when Legend deals damage
            DecisionYesNo = true;

            QuickHPStorage(baron);
            DealDamage(legend, baron, 2, DamageType.Melee);
            // 2 base + 1 from charge token = 3
            QuickHPCheck(-3);

            Assert.That(pool.CurrentValue, Is.EqualTo(0));
        }

        [Test()]
        public void TestDeclineToSpendToken()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var bounceBack = PlayCard("BounceBack");
            var pool = bounceBack.FindTokenPool("ChargePool");

            DealDamage(baron, legend, 2, DamageType.Melee);
            Assert.That(pool.CurrentValue, Is.EqualTo(1));

            // Decline to spend token
            DecisionYesNo = false;

            QuickHPStorage(baron);
            DealDamage(legend, baron, 2, DamageType.Melee);
            // Just 2 base damage
            QuickHPCheck(-2);

            // Token still there
            Assert.That(pool.CurrentValue, Is.EqualTo(1));
        }

        [Test()]
        public void TestNoTokensNoBoost()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var bounceBack = PlayCard("BounceBack");
            var pool = bounceBack.FindTokenPool("ChargePool");
            Assert.That(pool.CurrentValue, Is.EqualTo(0));

            // No tokens, no decision should be made, just normal damage
            QuickHPStorage(baron);
            DealDamage(legend, baron, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }
    }
}
