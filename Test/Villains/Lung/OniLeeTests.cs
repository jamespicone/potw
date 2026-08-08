using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Lung
{
    [TestFixture()]
    public class OniLeeTests : LungTestBase
    {
        [Test()]
        public void TestEndOfTurnDamagesLowestHero()
        {
            SetupLungGame();
            RemoveLungTriggers();

            var oniLee = PlayCard("OniLee");

            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 15);
            SetHitPoints(haka, 25);

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageSource(oniLee);
            AssertDamageType(DamageType.Melee);

            GoToEndOfTurn();

            QuickHPCheck(0, -3, 0);
        }

        [Test()]
        public void TestDamagePreventedWhenOneShotRevealed()
        {
            SetupLungGame();
            RemoveLungTriggers();

            var oniLee = PlayCard("OniLee");
            var smash = StackDeck("Smash");

            AssertNextRevealReveals(smash);
            DealDamage(legacy.CharacterCard, oniLee, 2, DamageType.Melee);

            AssertHitPoints(oniLee, 3);
        }

        [Test()]
        public void TestDamageAppliesWhenNonOneShotRevealed()
        {
            SetupLungGame();
            RemoveLungTriggers();

            var oniLee = PlayCard("OniLee");
            var wings = StackDeck("Wings");

            AssertNextRevealReveals(wings);
            DealDamage(legacy.CharacterCard, oniLee, 2, DamageType.Melee);

            AssertHitPoints(oniLee, 1);
        }
    }
}
