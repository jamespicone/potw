using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;
using Jp.ParahumansOfTheWormverse.Battery;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Battery
{
    [TestFixture()]
    public class BatteryTests : ParahumanTest
    {
        [Test()]
        public void TestUnchargedPower()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();

            AssertNumberOfUsablePowers(battery.CharacterCard, 1);

            Assert.IsFalse(battery.CharacterCardController.IsCharged(battery.CharacterCard));
            QuickHandStorage(battery);
            UsePower(battery.CharacterCard);
            QuickHandCheck(1);
            Assert.IsTrue(battery.CharacterCardController.IsCharged(battery.CharacterCard));

            AssertNumberOfUsablePowers(battery.CharacterCard, 1);
        }

        [Test()]
        public void TestChargedPower()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();

            AssertNumberOfUsablePowers(battery.CharacterCard, 1);

            Assert.IsFalse(battery.CharacterCardController.IsCharged(battery.CharacterCard));
            UsePower(battery.CharacterCard);
            Assert.IsTrue(battery.CharacterCardController.IsCharged(battery.CharacterCard));

            AssertNumberOfUsablePowers(battery.CharacterCard, 1);

            var magnetism = PutInHand("Magnetism");
            DecisionSelectCard = magnetism;
            AssertInHand(magnetism);
            UsePower(battery.CharacterCard);
            AssertIsInPlay(magnetism);

            Assert.IsFalse(battery.CharacterCardController.IsCharged(battery.CharacterCard));
            AssertNumberOfUsablePowers(battery.CharacterCard, 0);
        }
    }
}
