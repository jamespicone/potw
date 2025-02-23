using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Bitch
{
    [TestFixture()]
    public class BitchTests : ParahumanTest
    {
        [Test()]
        public void TestPowerNoDogs()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            QuickHPStorage(baron.CharacterCard, bitch.CharacterCard);

            UsePower(bitch.CharacterCard);

            QuickHPCheck(0, 0);
        }

        [Test()]
        public void TestPowerOneDog()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            var bastard = PlayCard("Bastard");

            QuickHPStorage(baron.CharacterCard, bitch.CharacterCard);

            AssertDamageType(DamageType.Melee);
            AssertDamageSource(bastard);
            DecisionSelectTarget = baron.CharacterCard;
            UsePower(bitch.CharacterCard);

            QuickHPCheck(-2, 0);
        }

        [Test()]
        public void TestPowerTwoDog()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            var bastard = PlayCard("Bastard");
            var milk = PlayCard("Milk");

            QuickHPStorage(baron.CharacterCard, bitch.CharacterCard);

            AssertDamageType(DamageType.Melee, DamageType.Melee);
            AssertDamageSource(milk, bastard);
            DecisionSelectCards = new Card[] { milk, baron.CharacterCard, bitch.CharacterCard };
            UsePower(bitch.CharacterCard);

            QuickHPCheck(-2, -2);
        }
    }
}
