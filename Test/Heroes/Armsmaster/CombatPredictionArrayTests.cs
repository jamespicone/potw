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
    public class CombatPredictionArrayTests : ParahumanTest
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

            var card = GetCard("CombatPredictionArray");
            Assert.That(card.DoKeywordsContain("equipment"), Is.True);
            Assert.That(card.DoKeywordsContain("module"), Is.True);
        }

        [Test()]
        public void TestPrimaryRevealsVillainCards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Primary";
            var cpa = PlayCard("CombatPredictionArray");

            DecisionActivateAbilities = new Card[] { cpa };

            // Should complete without crash - reveals top 3 villain cards and puts them back
            UsePower(halberd);
        }

        [Test()]
        public void TestSecondaryReducesVillainDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Secondary";
            var cpa = PlayCard("CombatPredictionArray");

            DecisionActivateAbilities = new Card[] { cpa };
            UsePower(halberd);

            // Now villain damage to Armsmaster should be reduced by 1
            QuickHPStorage(armsmaster);
            DealDamage(baron, armsmaster, 3, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestSecondaryDoesNotReduceHeroDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Secondary";
            var cpa = PlayCard("CombatPredictionArray");

            DecisionActivateAbilities = new Card[] { cpa };
            UsePower(halberd);

            // Hero damage to Armsmaster should NOT be reduced
            QuickHPStorage(armsmaster);
            DealDamage(bunker, armsmaster, 3, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestSecondaryExpiresAtStartOfNextTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Secondary";
            var cpa = PlayCard("CombatPredictionArray");

            DecisionActivateAbilities = new Card[] { cpa };
            UsePower(halberd);

            // Advance to start of Armsmaster's next turn so the status expires
            GoToStartOfTurn(armsmaster);

            // Villain damage should no longer be reduced
            QuickHPStorage(armsmaster);
            DealDamage(baron, armsmaster, 3, DamageType.Melee);
            QuickHPCheck(-3);
        }
    }
}
