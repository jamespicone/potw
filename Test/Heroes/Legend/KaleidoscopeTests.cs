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
    public class KaleidoscopeTests : ParahumanTest
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

            var card = GetCard("Kaleidoscope");
            Assert.That(card.DoKeywordsContain("laser"), Is.True);
            Assert.That(card.DoKeywordsContain("ongoing"), Is.True);
        }

        [Test()]
        public void TestEffectDealsSelectedDamageType()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var kaleidoscope = PlayCard("Kaleidoscope");

            DecisionActivateAbilities = new Card[] { kaleidoscope };
            DecisionSelectTarget = baron.CharacterCard;
            DecisionSelectDamageType = DamageType.Fire;

            AssertDamageType(DamageType.Fire);

            QuickHPStorage(baron);
            UsePower(legend);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestEffectDealsDifferentDamageType()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var kaleidoscope = PlayCard("Kaleidoscope");

            DecisionActivateAbilities = new Card[] { kaleidoscope };
            DecisionSelectTarget = baron.CharacterCard;
            DecisionSelectDamageType = DamageType.Cold;

            AssertDamageType(DamageType.Cold);

            QuickHPStorage(baron);
            UsePower(legend);
            QuickHPCheck(-2);
        }
    }
}
