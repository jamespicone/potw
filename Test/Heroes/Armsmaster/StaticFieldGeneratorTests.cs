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
    public class StaticFieldGeneratorTests : ParahumanTest
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

            var card = GetCard("StaticFieldGenerator");
            Assert.That(card.DoKeywordsContain("equipment"), Is.True);
            Assert.That(card.DoKeywordsContain("module"), Is.True);
        }

        [Test()]
        public void TestPrimaryDeals2LightningToNonHero()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Primary";
            var sfg = PlayCard("StaticFieldGenerator");

            DecisionSelectTarget = baron.CharacterCard;
            DecisionActivateAbilities = new Card[] { sfg };

            QuickHPStorage(baron);
            UsePower(halberd);
            // 2 lightning to Baron
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestSecondaryDeals1LightningToLowHPTargets()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            // Set Baron to 3 HP so secondary can hit
            SetHitPoints(baron.CharacterCard, 3);

            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Secondary";
            var sfg = PlayCard("StaticFieldGenerator");

            DecisionActivateAbilities = new Card[] { sfg };

            QuickHPStorage(baron);
            UsePower(halberd);
            // 1 lightning to Baron (3 HP <= 3)
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestSecondaryIgnoresHighHPTargets()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            // Baron starts at 40 HP, well above 3
            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Secondary";
            var sfg = PlayCard("StaticFieldGenerator");

            DecisionActivateAbilities = new Card[] { sfg };

            QuickHPStorage(baron);
            UsePower(halberd);
            // Baron has too much HP, no damage dealt
            QuickHPCheck(0);
        }
    }
}
