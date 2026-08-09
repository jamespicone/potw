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
    public class CurveshotTests : ParahumanTest
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

            var card = GetCard("Curveshot");
            Assert.That(card.DoKeywordsContain("laser"), Is.True);
            Assert.That(card.DoKeywordsContain("ongoing"), Is.True);
        }

        [Test()]
        public void TestPowerDealsDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var curveshot = PlayCard("Curveshot");

            // Use Curveshot's power - character card's effect is the only effect available, auto-selects
            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            UsePower(curveshot);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestPowerSurvivesBorrowedEffectAbility()
        {
            // Character Witness lends out the power on the hero character card the
            // Tribunal put into play from the box. Prime Wardens Fanatic's power ends
            // with "one hero may use a power", so Curveshot's power runs while the
            // Tribunal is still replacing hero character cards.
            SetupGameController(
                "BaronBlade",
                "Jp.ParahumansOfTheWormverse.Legend",
                "Bunker",
                "TheCelestialTribunal"
            );
            StartGame();

            RemoveMobileDefensePlatform();

            var curveshot = PlayCard("Curveshot");

            DecisionSelectFromBoxIdentifiers = new string[] { "PrimeWardensFanaticCharacter" };
            DecisionSelectFromBoxTurnTakerIdentifier = "Fanatic";
            PlayCard("RepresentativeOfEarth");
            ResetDecisions();

            PlayCard("CharacterWitness");

            // The borrowed power plays the top card of Legend's deck; pin it to
            // something harmless so only the effect's damage is measured.
            StackDeck("SkyHigh");

            // Legend uses the borrowed power, and takes the power use it grants.
            DecisionSelectCard = legend.CharacterCard;
            DecisionSelectTurnTaker = legend.TurnTaker;
            // The borrowed power isn't ours to name, so let its decision take its only
            // choice and name Curveshot for the power it grants.
            DecisionSelectPowers = new Card[] { null, curveshot };
            DecisionSelectTarget = baron.CharacterCard;

            GameController.OnMakeDecisions -= MakeDecisions;
            GameController.OnMakeDecisions += PickBorrowedEffectAbility;

            QuickHPStorage(baron);

            GoToStartOfTurn(env);

            // Only Legend's own effect is offered, and it happens.
            Assert.That(_effectAbilityChoices, Is.EqualTo(1));
            QuickHPCheck(-2);
        }

        private int _effectAbilityChoices = -1;

        // Every copy of the effect ability reports Legend's character card, so the
        // harness's card matching can't tell them apart. Take the last one offered.
        private IEnumerator PickBorrowedEffectAbility(IDecision decision)
        {
            if (decision is ActivateAbilityDecision activate)
            {
                _effectAbilityChoices = activate.Choices.Count();
                activate.SelectedAbility = activate.Choices.Last();
                yield break;
            }

            GameController.ExhaustCoroutine(MakeDecisions(decision));
            yield break;
        }

        [Test()]
        public void TestPowerDamageIsIrreducible()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var curveshot = PlayCard("Curveshot");

            DecisionSelectTarget = baron.CharacterCard;

            AssertIrreducible();

            UsePower(curveshot);
        }
    }
}
