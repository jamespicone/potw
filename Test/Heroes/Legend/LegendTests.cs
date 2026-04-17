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
    public class LegendTests : ParahumanTest
    {
        [Test()]
        public void TestLegendHP()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
            StartGame();

            Assert.That(legend.CharacterCard.MaximumHitPoints, Is.EqualTo(28));
        }

        [Test()]
        public void TestPowerDeals2EnergyDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            // With only the character card's "effect" ability available, it auto-selects
            DecisionSelectTarget = baron.CharacterCard;

            AssertDamageSource(legend.CharacterCard);
            AssertDamageType(DamageType.Energy);

            QuickHPStorage(baron);
            UsePower(legend);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestIncapDealsDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();
            IncapacitateCharacter(legend.CharacterCard, baron.CharacterCard);

            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            UseIncapacitatedAbility(legend, 0);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestIncapUsePower()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();
            IncapacitateCharacter(legend.CharacterCard, baron.CharacterCard);

            // Select Bunker to use a power
            DecisionSelectTurnTaker = bunker.TurnTaker;

            UseIncapacitatedAbility(legend, 1);
        }

        [Test()]
        public void TestIncapDrawCard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            IncapacitateCharacter(legend.CharacterCard, baron.CharacterCard);

            // Select Bunker to draw
            DecisionSelectTurnTaker = bunker.TurnTaker;

            QuickHandStorage(bunker);
            UseIncapacitatedAbility(legend, 2);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestGuiseEffects()
        {
            SetupGameController("BaronBlade", "Guise", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            var kaleidoscope = PlayCard("Kaleidoscope");
            var splitshot = PlayCard("Splitshot");

            GoToPlayCardPhase(guise);

            DecisionSelectTurnTaker = legend.TurnTaker;
            var uyitg = PlayCard("UhYeahImThatGuy");
            ResetDecisions();

            DecisionSelectTargets = new Card[] { baron.CharacterCard, null };
            DecisionSelectDamageType = DamageType.Infernal;

            AssertDamageSource(guise.CharacterCard);
            AssertDamageType(DamageType.Infernal);

            QuickHPStorage(baron);
            UsePower(uyitg);
            QuickHPCheck(-2);
        }

        // TODO: Guise copying Legend's powers crashes when accessing effects.
        // Check CardWithoutReplacements on source or something?
    }
}
