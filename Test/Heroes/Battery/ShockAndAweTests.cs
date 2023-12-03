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
    public class ShockAndAweTests : ParahumanTest
    {
        [Test()]
        public void TestUncharged()
        {
            SetupGameController("AkashBhuta", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();

            var slide = PlayCard("LivingRockslide");

            DecisionSelectTarget = akash.CharacterCard;

            AssertDamageSource(battery.CharacterCard);
            AssertDamageType(DamageType.Lightning);

            QuickHPStorage(akash.CharacterCard, slide);
            PlayCard("ShockAndAwe");
            QuickHPCheck(-2, 0);
        }

        [Test()]
        public void TestChargedCantRepeatTarget()
        {
            SetupGameController("AkashBhuta", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();

            UsePower(battery);

            AssertDamageSource(battery.CharacterCard);
            AssertDamageType(DamageType.Lightning);

            QuickHPStorage(akash.CharacterCard);
            PlayCard("ShockAndAwe");
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestCharged()
        {
            SetupGameController("AkashBhuta", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();

            UsePower(battery);

            var slide = PlayCard("LivingRockslide");
            var slide2 = PlayCard("LivingRockslide");
            var phalanges = PlayCard("ArborealPhalanges");

            DecisionSelectTargets = new Card[] { akash.CharacterCard, slide, slide2, phalanges, battery.CharacterCard };

            AssertDamageSource(battery.CharacterCard);
            AssertDamageType(DamageType.Lightning);

            QuickHPStorage(akash.CharacterCard, slide, slide2, phalanges, battery.CharacterCard);
            PlayCard("ShockAndAwe");
            QuickHPCheck(-2, -3, -3, -3, 0);
        }
    }
}
