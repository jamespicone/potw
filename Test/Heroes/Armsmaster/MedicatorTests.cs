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
    public class MedicatorTests : ParahumanTest
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

            var card = GetCard("Medicator");
            Assert.That(card.DoKeywordsContain("equipment"), Is.True);
            Assert.That(card.DoKeywordsContain("module"), Is.True);
        }

        [Test()]
        public void TestPrimaryAllHeroesRegain1HP()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            SetHitPoints(armsmaster, 20);
            SetHitPoints(bunker, 20);

            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Primary";
            var medicator = PlayCard("Medicator");

            DecisionActivateAbilities = new Card[] { medicator };

            QuickHPStorage(armsmaster, bunker);
            UsePower(halberd);
            QuickHPCheck(1, 1);
        }

        [Test()]
        public void TestSecondaryArmsmasterRegains2HP()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            SetHitPoints(armsmaster, 20);

            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Secondary";
            var medicator = PlayCard("Medicator");

            DecisionActivateAbilities = new Card[] { medicator };

            QuickHPStorage(armsmaster);
            UsePower(halberd);
            QuickHPCheck(2);
        }
    }
}
