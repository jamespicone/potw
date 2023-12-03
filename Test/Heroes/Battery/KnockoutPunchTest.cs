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
    public class KnockoutPunchTests : ParahumanTest
    {
        [Test()]
        public void TestUncharged()
        {
            SetupGameController("AkashBhuta", "Jp.ParahumansOfTheWormverse.Battery", "Haka", "Legacy", "InsulaPrimalis");

            StartGame();

            var slide = PlayCard("LivingRockslide");

            QuickHPStorage(slide, battery.CharacterCard, haka.CharacterCard, legacy.CharacterCard);
            DecisionSelectTargets = new Card[] { slide, battery.CharacterCard, haka.CharacterCard, legacy.CharacterCard };

            AssertDamageSource(battery.CharacterCard);
            AssertDamageType(DamageType.Lightning);
            PlayCard("KnockoutPunch");

            QuickHPCheck(-2, 0, 0, 0);

            GoToStartOfTurn(battery);

            QuickHPCheck(0, -1, -1, -1);
        }

        [Test()]
        public void TestCharged()
        {
            SetupGameController("AkashBhuta", "Jp.ParahumansOfTheWormverse.Battery", "Haka", "Legacy", "InsulaPrimalis");

            StartGame();

            UsePower(battery.CharacterCard);

            var slide = PlayCard("LivingRockslide");

            QuickHPStorage(slide, battery.CharacterCard, haka.CharacterCard, legacy.CharacterCard);
            DecisionSelectTargets = new Card[] { slide, battery.CharacterCard, haka.CharacterCard, legacy.CharacterCard };

            AssertDamageSource(battery.CharacterCard);
            AssertDamageType(DamageType.Lightning);
            PlayCard("KnockoutPunch");

            QuickHPCheck(-2, 0, 0, 0);

            GoToStartOfTurn(battery);

            QuickHPCheck(0, 0, 0, 0);
        }

        [Test()]
        public void TestChargedAgainstCharacter()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Battery", "Haka", "Legacy", "InsulaPrimalis");

            StartGame();

            DestroyCard(baron.CharacterCard);

            UsePower(battery.CharacterCard);

            QuickHPStorage(baron.CharacterCard, battery.CharacterCard, haka.CharacterCard, legacy.CharacterCard);
            DecisionSelectTargets = new Card[] { baron.CharacterCard, battery.CharacterCard, haka.CharacterCard, legacy.CharacterCard };

            AssertDamageSource(battery.CharacterCard);
            AssertDamageType(DamageType.Lightning);
            PlayCard("KnockoutPunch");

            QuickHPCheck(-2, 0, 0, 0);

            GoToStartOfTurn(battery);

            QuickHPCheck(0, 0, -3, 0);
        }
    }
}
