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

            Assert.That(battery.CharacterCardController.IsCharged(battery.CharacterCard), Is.False);
            QuickHandStorage(battery);
            UsePower(battery.CharacterCard);
            QuickHandCheck(1);
            Assert.That(battery.CharacterCardController.IsCharged(battery.CharacterCard), Is.True);

            AssertNumberOfUsablePowers(battery.CharacterCard, 1);
        }

        [Test()]
        public void TestChargedPower()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();

            AssertNumberOfUsablePowers(battery.CharacterCard, 1);

            Assert.That(battery.CharacterCardController.IsCharged(battery.CharacterCard), Is.False);
            UsePower(battery.CharacterCard);
            Assert.That(battery.CharacterCardController.IsCharged(battery.CharacterCard), Is.True);

            AssertNumberOfUsablePowers(battery.CharacterCard, 1);

            var magnetism = PutInHand("Magnetism");
            DecisionSelectCard = magnetism;
            AssertInHand(magnetism);
            UsePower(battery.CharacterCard);
            AssertIsInPlay(magnetism);

            Assert.That(battery.CharacterCardController.IsCharged(battery.CharacterCard), Is.False);
            AssertNumberOfUsablePowers(battery.CharacterCard, 0);
        }
    }
}
