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
    public class SpotTheFlawTests : ParahumanTest
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

            var card = GetCard("SpotTheFlaw");
            Assert.That(card.DoKeywordsContain("ongoing"), Is.True);
        }

        [Test()]
        public void TestPlayDealsSelfDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            QuickHPStorage(tattletale);
            PlayCard("SpotTheFlaw");
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestMakesHeroDamageIrreducible()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            // Give Baron damage reduction
            PlayCard("LivingForceField");

            PlayCard("SpotTheFlaw");

            // Hero damage should be irreducible, ignoring LivingForceField's reduction
            QuickHPStorage(baron);
            DealDamage(bunker, baron, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestDestroysItselfAtStartOfTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            var card = PlayCard("SpotTheFlaw");
            AssertIsInPlay(card);

            GoToStartOfTurn(tattletale);
            AssertInTrash(card);
        }

        [Test()]
        public void TestDoesNotAffectVillainDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            // Give Bunker damage reduction
            PlayCard("HeavyPlating");

            PlayCard("SpotTheFlaw");

            // Villain damage should still be reduced by HeavyPlating (1 reduction)
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 2, DamageType.Melee);
            QuickHPCheck(-1);
        }
    }
}
