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
    public class AcceleratorTests : ParahumanTest
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

            var card = GetCard("Accelerator");
            Assert.That(card.DoKeywordsContain("equipment"), Is.True);
            Assert.That(card.DoKeywordsContain("module"), Is.True);
        }

        [Test()]
        public void TestPrimaryAllHeroesMayDraw()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Primary";
            var accelerator = PlayCard("Accelerator");

            DecisionActivateAbilities = new Card[] { accelerator };

            QuickHandStorage(armsmaster, bunker);
            UsePower(halberd);
            // Survey draws 1, Accelerator primary lets all heroes draw 1
            // Armsmaster: +1 (survey) +1 (accel) = +2, Bunker: +1 (accel)
            QuickHandCheck(2, 1);
        }

        [Test()]
        public void TestSecondaryNonArmsmasterPlays()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var bunkerCard = PutInHand("FlakCannon");

            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Secondary";
            var accelerator = PlayCard("Accelerator");

            DecisionSelectTurnTaker = bunker.TurnTaker;
            DecisionSelectCard = bunkerCard;
            DecisionActivateAbilities = new Card[] { accelerator };

            UsePower(halberd);

            AssertIsInPlay(bunkerCard);
        }
    }
}
