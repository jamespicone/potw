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
    public class EnergyConversionTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
        }

        [Test()]
        public void TestIsOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
            StartGame();

            var card = GetCard("EnergyConversion");
            Assert.That(card.DoKeywordsContain("ongoing"), Is.True);
        }

        [Test()]
        public void TestDrawCardOnVillainDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
            StartGame();

            PlayCard("EnergyConversion");

            // DrawACardOrPlayACard: function 0 = "Play a card", function 1 = "Draw a card"
            DecisionSelectFunction = 1;

            QuickHandStorage(legend);
            DealDamage(baron, legend, 2, DamageType.Melee);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestPlayCardOnVillainDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
            StartGame();

            PlayCard("EnergyConversion");

            var cardToPlay = PutInHand("SkyHigh");

            // DrawACardOrPlayACard: function 0 = "Play a card", function 1 = "Draw a card"
            DecisionSelectFunction = 0;
            DecisionSelectCard = cardToPlay;

            DealDamage(baron, legend, 2, DamageType.Melee);

            AssertIsInPlay(cardToPlay);
        }

        [Test()]
        public void TestTriggersOnEnvironmentDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
            StartGame();

            PlayCard("EnergyConversion");

            var envTarget = PlayCard("VelociraptorPack");

            // DrawACardOrPlayACard: function 0 = "Play a card", function 1 = "Draw a card"
            DecisionSelectFunction = 1;

            QuickHandStorage(legend);
            DealDamage(envTarget, legend, 2, DamageType.Melee);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestDoesNotTriggerOnHeroDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            PlayCard("EnergyConversion");

            QuickHandStorage(legend);
            DealDamage(bunker, legend, 2, DamageType.Melee);
            // No trigger from hero damage
            QuickHandCheck(0);
        }

        [Test()]
        public void TestDoesNotTriggerWhenNoDamageDealt()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
            StartGame();

            PlayCard("EnergyConversion");

            QuickHandStorage(legend);
            // Deal 0 damage - shouldn't trigger since DidDealDamage would be false
            DealDamage(baron, legend, 0, DamageType.Melee);
            QuickHandCheck(0);
        }
    }
}
