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
    public class GrappleTests : ParahumanTest
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

            var card = GetCard("Grapple");
            Assert.That(card.DoKeywordsContain("equipment"), Is.True);
            Assert.That(card.DoKeywordsContain("module"), Is.True);
        }

        [Test()]
        public void TestPrimaryDeals1ProjectileTo2Targets()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var envCard = PlayCard("VelociraptorPack");

            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Primary";
            var grapple = PlayCard("Grapple");

            DecisionSelectTargets = new Card[] { baron.CharacterCard, envCard };
            DecisionActivateAbilities = new Card[] { grapple };

            QuickHPStorage(baron);
            UsePower(halberd);
            // 1 projectile to Baron
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestSecondaryDestroysOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var ongoing = PlayCard("LivingForceField");

            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Secondary";
            var grapple = PlayCard("Grapple");

            DecisionSelectCard = ongoing;
            DecisionActivateAbilities = new Card[] { grapple };

            UsePower(halberd);

            AssertInTrash(ongoing);
        }
    }
}
