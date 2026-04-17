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
    public class ExperimentalHalberdTests : ParahumanTest
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

            var card = GetCard("ExperimentalHalberd");
            Assert.That(card.DoKeywordsContain("equipment"), Is.True);
            Assert.That(card.DoKeywordsContain("halberd"), Is.True);
        }

        [Test()]
        public void TestPowerDeals1IrreducibleEnergy()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var halberd = PlayCard("ExperimentalHalberd");
            DecisionSelectTarget = baron.CharacterCard;
            DecisionDoNotActivatableAbility = true;

            QuickHPStorage(baron);
            UsePower(halberd);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestPowerDamageIsIrreducible()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            // Play a damage reduction card on Baron
            PlayCard("LivingForceField");

            var halberd = PlayCard("ExperimentalHalberd");
            DecisionSelectTarget = baron.CharacterCard;
            DecisionDoNotActivatableAbility = true;

            QuickHPStorage(baron);
            UsePower(halberd);
            // Still deals 1 because irreducible
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestPowerActivatesModules()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var halberd = PlayCard("ExperimentalHalberd");
            DecisionSelectWord = "Primary";
            var flamethrower = PlayCard("Flamethrower");
            AssertNextToCard(flamethrower, halberd);

            DecisionSelectTarget = baron.CharacterCard;
            DecisionActivateAbilities = new Card[] { flamethrower };

            QuickHPStorage(baron);
            UsePower(halberd);
            // 1 energy + 2 fire = 3
            QuickHPCheck(-3);
        }
    }
}
