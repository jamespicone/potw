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
    public class SurveyHalberdTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();
        }

        [Test()]
        public void TestIsEquipmentHalberd()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var card = GetCard("SurveyHalberd");
            Assert.That(card.DoKeywordsContain("equipment"), Is.True);
            Assert.That(card.DoKeywordsContain("halberd"), Is.True);
        }

        [Test()]
        public void TestPowerDrawsCard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var halberd = PlayCard("SurveyHalberd");
            DecisionDoNotActivatableAbility = true;

            QuickHandStorage(armsmaster);
            UsePower(halberd);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestPowerActivatesModules()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Secondary";
            var medicator = PlayCard("Medicator");
            AssertNextToCard(medicator, halberd);

            SetHitPoints(armsmaster, 20);

            DecisionActivateAbilities = new Card[] { medicator };

            QuickHPStorage(armsmaster);
            QuickHandStorage(armsmaster);
            UsePower(halberd);
            // Draws a card and Medicator secondary heals Armsmaster 2
            QuickHandCheck(1);
            QuickHPCheck(2);
        }
    }
}
