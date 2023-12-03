using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Battery
{
    [TestFixture()]
    public class KnockEmDownTests : ParahumanTest
    {
        [Test()]
        public void TestUncharged()
        {
            SetupGameController("AkashBhuta", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();

            var phalanges = PlayCard("ArborealPhalanges");
            var slide = PlayCard("LivingRockslide");

            DecisionSelectTargets = new Card[] { phalanges, phalanges, akash.CharacterCard, slide };

            QuickHPStorage(phalanges, slide, akash.CharacterCard);
            AssertDamageSource(battery.CharacterCard);
            AssertDamageType(DamageType.Melee);
            PlayCard("KnockEmDown");
            QuickHPCheck(-2, 0, 0);
        }

        [Test()]
        public void TestCharged()
        {
            SetupGameController("AkashBhuta", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();

            UsePower(battery.CharacterCard);

            var phalanges = PlayCard("ArborealPhalanges");
            var slide = PlayCard("LivingRockslide");

            DecisionSelectTargets = new Card[] { phalanges, phalanges, akash.CharacterCard, slide };

            QuickHPStorage(phalanges, slide, akash.CharacterCard);
            AssertDamageSource(battery.CharacterCard);
            AssertDamageType(DamageType.Melee);
            PlayCard("KnockEmDown");
            QuickHPCheck(-4, -2, -2);
        }
    }
}
