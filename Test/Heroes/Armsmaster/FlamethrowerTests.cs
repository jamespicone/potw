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
    public class FlamethrowerTests : ParahumanTest
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

            var card = GetCard("Flamethrower");
            Assert.That(card.DoKeywordsContain("equipment"), Is.True);
            Assert.That(card.DoKeywordsContain("module"), Is.True);
        }

        [Test()]
        public void TestPrimaryDeals2FireToNonHero()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            // Use SurveyHalberd so the halberd power itself doesn't deal damage
            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Primary";
            var flamethrower = PlayCard("Flamethrower");

            DecisionSelectTarget = baron.CharacterCard;
            DecisionActivateAbilities = new Card[] { flamethrower };

            QuickHPStorage(baron);
            UsePower(halberd);
            // Survey draws a card, then Flamethrower primary deals 2 fire
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestSecondaryDestroysEnvironmentCard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var envCard = PlayCard("VelociraptorPack");

            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Secondary";
            var flamethrower = PlayCard("Flamethrower");

            DecisionSelectCard = envCard;
            DecisionActivateAbilities = new Card[] { flamethrower };

            UsePower(halberd);

            AssertInTrash(envCard);
        }
    }
}
