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
    public class TargetingComputerTests : ParahumanTest
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

            var card = GetCard("TargetingComputer");
            Assert.That(card.DoKeywordsContain("equipment"), Is.True);
            Assert.That(card.DoKeywordsContain("module"), Is.True);
        }

        [Test()]
        public void TestPrimaryIncreasesAndMakesIrreducible()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Primary";
            var tc = PlayCard("TargetingComputer");

            // Select Bunker as the hero target to buff
            DecisionSelectCard = bunker.CharacterCard;
            DecisionActivateAbilities = new Card[] { tc };
            UsePower(halberd);

            // Now Bunker's next damage is +1 and irreducible
            // Add damage reduction to verify irreducible
            PlayCard("LivingForceField");

            QuickHPStorage(baron);
            DealDamage(bunker, baron, 2, DamageType.Melee);
            // 2 + 1 (increase) = 3, irreducible, so Living Force Field doesn't reduce
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestPrimaryIsOneUse()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Primary";
            var tc = PlayCard("TargetingComputer");

            DecisionSelectCard = bunker.CharacterCard;
            DecisionActivateAbilities = new Card[] { tc };
            UsePower(halberd);

            // First hit is buffed
            QuickHPStorage(baron);
            DealDamage(bunker, baron, 2, DamageType.Melee);
            QuickHPCheck(-3);

            // Second hit is normal
            QuickHPStorage(baron);
            DealDamage(bunker, baron, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestSecondaryDeals4ProjectileNextTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Secondary";
            var tc = PlayCard("TargetingComputer");

            DecisionSelectTarget = baron.CharacterCard;
            DecisionActivateAbilities = new Card[] { tc };
            UsePower(halberd);

            // At the start of Armsmaster's next turn, deals 4 projectile
            QuickHPStorage(baron);
            GoToStartOfTurn(armsmaster);
            QuickHPCheck(-4);
        }
    }
}
