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
    public class PowTests : ParahumanTest
    {
        [Test()]
        public void TestUncharged()
        {
            SetupGameController("AkashBhuta", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();

            QuickHPStorage(akash.CharacterCard);
            DecisionSelectTarget = akash.CharacterCard;
            
            AssertDamageSource(battery.CharacterCard);
            AssertDamageType(DamageType.Melee);

            PlayCard("Pow");
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestCharged()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();

            UsePower(battery);

            QuickHPStorage(baron.CharacterCard);
            DecisionSelectCard = GetMobileDefensePlatform().Card;
            DecisionSelectTarget = baron.CharacterCard;

            AssertDamageSource(battery.CharacterCard);
            AssertDamageType(DamageType.Melee);

            PlayCard("Pow");
            QuickHPCheck(-3);
        }
    }
}
