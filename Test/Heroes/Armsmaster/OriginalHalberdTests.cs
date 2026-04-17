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
    public class OriginalHalberdTests : ParahumanTest
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

            var card = GetCard("OriginalHalberd");
            Assert.That(card.DoKeywordsContain("equipment"), Is.True);
            Assert.That(card.DoKeywordsContain("halberd"), Is.True);
        }

        [Test()]
        public void TestPowerDeals2Melee()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var halberd = PlayCard("OriginalHalberd");
            DecisionSelectTarget = baron.CharacterCard;
            DecisionDoNotActivatableAbility = true;

            QuickHPStorage(baron);
            UsePower(halberd);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestPowerActivatesPrimaryModule()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var halberd = PlayCard("OriginalHalberd");
            DecisionSelectWord = "Primary";
            var flamethrower = PlayCard("Flamethrower");
            AssertNextToCard(flamethrower, halberd);

            // Power deals 2 melee, then Flamethrower primary deals 2 fire to non-hero
            DecisionSelectTarget = baron.CharacterCard;
            DecisionActivateAbilities = new Card[] { flamethrower };

            QuickHPStorage(baron);
            UsePower(halberd);
            QuickHPCheck(-4);
        }

        [Test()]
        public void TestPowerActivatesSecondaryModule()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var halberd = PlayCard("OriginalHalberd");
            DecisionSelectWord = "Secondary";
            var medicator = PlayCard("Medicator");
            AssertNextToCard(medicator, halberd);

            SetHitPoints(armsmaster, 20);

            // Power deals 2 melee, then Medicator secondary heals Armsmaster 2
            DecisionSelectTarget = baron.CharacterCard;
            DecisionActivateAbilities = new Card[] { medicator };

            QuickHPStorage(baron, armsmaster);
            UsePower(halberd);
            QuickHPCheck(-2, 2);
        }

        [Test()]
        public void TestPowerWithBothModules()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var halberd = PlayCard("OriginalHalberd");
            DecisionSelectWord = "Primary";
            var flamethrower = PlayCard("Flamethrower");

            // Second module auto-assigns as Secondary
            var medicator = PlayCard("Medicator");
            AssertNextToCard(flamethrower, halberd);
            AssertNextToCard(medicator, halberd);

            SetHitPoints(armsmaster, 20);

            // Power deals 2 melee, Flamethrower primary deals 2 fire, Medicator secondary heals 2
            DecisionSelectTarget = baron.CharacterCard;
            DecisionActivateAbilities = new Card[] { flamethrower, medicator };

            QuickHPStorage(baron, armsmaster);
            UsePower(halberd);
            QuickHPCheck(-4, 2);
        }
    }
}
