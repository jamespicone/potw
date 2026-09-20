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

        // The Celestial Tribunal's Representative of Earth puts our character card into play
        // owned by the environment: no HeroTurnTakerController, no CharacterCard, and no deck,
        // hand or trash. Legend's character card is the only Effect provider that can be in play
        // there, so it is chosen automatically.
        [Test()]
        public void TestBroughtInByRepresentativeOfEarth()
        {
            SetupGameController("BaronBlade", "Legacy", "TheCelestialTribunal");
            StartGame();
            RemoveMobileDefensePlatform();

            var legendCard = SummonRepresentativeOfEarth("Legend", "LegendCharacter");

            GoToStartOfTurn(legacy);
            GoToStartOfTurn(env);
            GoToStartOfTurn(baron);
            AssertIsInPlay(legendCard);

            // The damage comes from the summoned card - CharacterCard is null here, so the effects
            // fall back to the card providing them, which is Legend himself.
            DecisionSelectTarget = baron.CharacterCard;
            AssertDamageSource(legendCard);
            AssertDamageType(DamageType.Energy);

            QuickHPStorage(baron);
            UsePower(legendCard, 0);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestPowerLentByCalledToJudgement()
        {
            SetupGameController("BaronBlade", "Legacy", "TheCelestialTribunal");
            StartGame();
            RemoveMobileDefensePlatform();

            SummonRepresentativeOfEarth("Legend", "LegendCharacter");

            // CharacterCard follows the replacement, so the damage comes from Legacy - and Legacy
            // is Baron Blade's nemesis, so his 2 damage lands as 3.
            DecisionSelectTarget = baron.CharacterCard;
            AssertDamageSource(legacy.CharacterCard);

            QuickHPStorage(baron);
            UsePowerLentByCalledToJudgement(legacy.CharacterCard);
            QuickHPCheck(-3);
        }
    }
}
