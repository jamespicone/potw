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
    public class RecyclerTests : ParahumanTest
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

            var card = GetCard("Recycler");
            Assert.That(card.DoKeywordsContain("equipment"), Is.True);
            Assert.That(card.DoKeywordsContain("module"), Is.True);
        }

        [Test()]
        public void TestPrimaryDrawsCard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Primary";
            var recycler = PlayCard("Recycler");

            DecisionActivateAbilities = new Card[] { recycler };

            QuickHandStorage(armsmaster);
            UsePower(halberd);
            // Survey draws 1 + Recycler primary draws 1 = +2
            QuickHandCheck(2);
        }

        [Test()]
        public void TestSecondaryActivatesOtherModuleSecondary()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            SetHitPoints(armsmaster, 20);

            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Primary";
            var medicator = PlayCard("Medicator");

            // Recycler auto-assigns as Secondary
            var recycler = PlayCard("Recycler");

            // Primary activation: Medicator, Secondary activation: Recycler, Recycler's secondary activates Medicator's secondary
            DecisionActivateAbilities = new Card[] { medicator, recycler, medicator };

            QuickHPStorage(armsmaster);
            UsePower(halberd);
            // Medicator primary heals all heroes 1 + Medicator secondary (via Recycler) heals Armsmaster 2
            QuickHPCheck(3);
        }

        [Test()]
        public void TestSecondaryWithNoOtherModule()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Secondary";
            var recycler = PlayCard("Recycler");

            DecisionActivateAbilities = new Card[] { recycler };

            // Should not crash even with no other module
            UsePower(halberd);
        }
    }
}
