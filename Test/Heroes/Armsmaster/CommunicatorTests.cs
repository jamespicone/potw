using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Armsmaster
{
    [TestFixture()]
    public class CommunicatorTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();
        }

        [Test()]
        public void TestIsEquipmentModule()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var card = GetCard("Communicator");
            Assert.That(card.DoKeywordsContain("equipment"), Is.True);
            Assert.That(card.DoKeywordsContain("module"), Is.True);
        }

        [Test()]
        public void TestPrimaryNonArmsmasterUsesPower()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Primary";
            var communicator = PlayCard("Communicator");

            DecisionSelectTurnTaker = bunker.TurnTaker;
            DecisionActivateAbilities = new Card[] { communicator };

            // Bunker's base power draws a card
            QuickHandStorage(bunker);
            UsePower(halberd);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestSecondaryNonArmsmasterPlaysCard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var bunkerCard = PutInHand("FlakCannon");

            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Secondary";
            var communicator = PlayCard("Communicator");

            DecisionSelectTurnTaker = bunker.TurnTaker;
            DecisionSelectCard = bunkerCard;
            DecisionActivateAbilities = new Card[] { communicator };

            UsePower(halberd);

            AssertIsInPlay(bunkerCard);
        }
    }
}
