using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Legend
{
    [TestFixture()]
    public class FreezeblastTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
        }

        [Test()]
        public void TestIsLaser()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
            StartGame();

            var card = GetCard("Freezeblast");
            Assert.That(card.DoKeywordsContain("laser"), Is.True);
            Assert.That(card.DoKeywordsContain("ongoing"), Is.True);
        }

        [Test()]
        public void TestEffectDealsColdDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var freezeblast = PlayCard("Freezeblast");

            // Select Freezeblast's effect when using Legend's power
            DecisionActivateAbilities = new Card[] { freezeblast };
            DecisionSelectTarget = baron.CharacterCard;

            AssertDamageType(DamageType.Cold);

            QuickHPStorage(baron);
            UsePower(legend);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestEffectReducesTargetDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var freezeblast = PlayCard("Freezeblast");

            // Hit Baron with Freezeblast effect
            DecisionActivateAbilities = new Card[] { freezeblast };
            DecisionSelectTarget = baron.CharacterCard;
            UsePower(legend);

            // Baron should deal 1 less damage until start of Legend's next turn
            QuickHPStorage(legend);
            DealDamage(baron, legend, 3, DamageType.Melee);
            QuickHPCheck(-2); // 3 - 1 reduction = 2
        }
    }
}
