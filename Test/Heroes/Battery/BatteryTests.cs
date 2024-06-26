using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;
using Jp.ParahumansOfTheWormverse.Battery;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller.TheCelestialTribunal;
using System.Runtime.InteropServices;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Battery
{
    [TestFixture()]
    public class BatteryTests : ParahumanTest
    {
        [Test()]
        public void TestUnchargedPower()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();

            AssertNumberOfUsablePowers(battery.CharacterCard, 1);

            Assert.That(battery.CharacterCardController.IsCharged(battery.CharacterCard), Is.False);
            QuickHandStorage(battery);
            UsePower(battery.CharacterCard);
            QuickHandCheck(1);
            Assert.That(battery.CharacterCardController.IsCharged(battery.CharacterCard), Is.True);

            AssertNumberOfUsablePowers(battery.CharacterCard, 1);
        }

        [Test()]
        public void TestChargedPower()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();

            AssertNumberOfUsablePowers(battery.CharacterCard, 1);

            Assert.That(battery.CharacterCardController.IsCharged(battery.CharacterCard), Is.False);
            UsePower(battery.CharacterCard);
            Assert.That(battery.CharacterCardController.IsCharged(battery.CharacterCard), Is.True);

            AssertNumberOfUsablePowers(battery.CharacterCard, 1);

            var magnetism = PutInHand("Magnetism");
            DecisionSelectCard = magnetism;
            AssertInHand(magnetism);
            UsePower(battery.CharacterCard);
            AssertIsInPlay(magnetism);

            Assert.That(battery.CharacterCardController.IsCharged(battery.CharacterCard), Is.False);
            AssertNumberOfUsablePowers(battery.CharacterCard, 0);
        }

        [Test()]
        public void TestSwitchingCharacterCardsDoesntUncharge()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Battery", "Guise/CompletionistGuiseCharacter", "InsulaPrimalis");

            StartGame();

            UsePower(battery.CharacterCard);
            Assert.That(battery.CharacterCardController.IsCharged(battery.CharacterCard), Is.True);

            DecisionSelectCards = new Card[] { battery.CharacterCard, battery.CharacterCard };
            UsePower(guise);

            Assert.That(battery.CharacterCardController.IsCharged(battery.CharacterCard), Is.True);
            Assert.That(battery.CharacterCard.PromoIdentifierOrIdentifier, Is.EqualTo("BatteryCauldronCapeCharacter"));
        }

        [Test()]
        public void GetBothCharges()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Battery", "TheCelestialTribunal");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            //UsePower(battery.CharacterCard);
            //Assert.That(battery.CharacterCardController.IsCharged(battery.CharacterCard), Is.True);

            //DecisionSelectFromBoxIdentifiers = new string[] { "Jp.ParahumansOfTheWormverse.BatteryCauldronCapeCharacter" };
            //DecisionSelectFromBoxTurnTakerIdentifier = "Jp.ParahumansOfTheWormverse.Battery";

            DecisionSelectFromBoxIdentifiers = new string[] { "TachyonCharacter" };
            DecisionSelectFromBoxTurnTakerIdentifier = "Tachyon";
            DecisionSelectCard = battery.CharacterCard;
            //DecisionSelectPowerIndex = 1;
            PlayCard("CalledToJudgement");

            

            Log.Debug("Trying to use power directly");

            var rep = FindCard(c => c.Location.IsPlayAreaOf(env.TurnTaker) && c.IsHeroCharacterCard);
            var repOfEarth = FindCard(c => c.Identifier == "RepresentativeOfEarth");
            Log.Debug($"Rep: {rep}");
            var e = GameController.SelectAndUsePower(
                battery,
                optional: true,
                (Power power) => power.CardSource != null && power.CardSource.Card == rep,
                1,
                eliminateUsedPowers: false,
                null,
                showMessage: false,
                allowAnyHeroPower: true,
                allowReplacements: true,
                canBeCancelled: true,
                null,
                forceDecision: false,
                allowOutOfPlayPower: false,
                FindCardController(repOfEarth).GetCardSource()
            );
            RunCoroutine(e);

            IEnumerable<Power> possiblePowers =
                GameController.GetUsablePowersThisTurn(
                    battery,
                    eliminateUsedPowers: false,
                    allowAnyHeroPower: true,
                    allowReplacements: true,
                    canBeCancelled: true,
                    allowOutOfPlayPower: false,
                    null,
                    battery.CharacterCardController.GetCardSource()
                ).Where(p => p.CardSource != null & p.CardSource.Card == rep);

            Log.Debug("Powers:");
            foreach (var p in possiblePowers)
            { Log.Debug($"\t{p}"); }
        }

        [Test()]
        public void LegacyJudgement()
        {
            SetupGameController("BaronBlade", "Legacy", "TheCelestialTribunal");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            DecisionSelectFromBoxIdentifiers = new string[] { "TachyonCharacter" };
            DecisionSelectFromBoxTurnTakerIdentifier = "Tachyon";
            DecisionSelectCard = legacy.CharacterCard;
            PlayCard("CalledToJudgement");
        }

        [Test()]
        public void BatteryJudgement()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Battery", "TheCelestialTribunal");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            DecisionSelectFromBoxIdentifiers = new string[] { "TachyonCharacter" };
            DecisionSelectFromBoxTurnTakerIdentifier = "Tachyon";
            DecisionSelectCard = battery.CharacterCard;
            PlayCard("CalledToJudgement");

            Log.Debug("Trying to use power directly");

            var rep = FindCard(c => c.Location.IsPlayAreaOf(env.TurnTaker) && c.IsHeroCharacterCard);
            var repOfEarth = FindCard(c => c.Identifier == "RepresentativeOfEarth");
            Log.Debug($"Rep: {rep}");
            var e = GameController.SelectAndUsePower(
                battery,
                optional: true,
                (Power power) => power.CardSource != null && power.CardSource.Card == rep,
                1,
                eliminateUsedPowers: false,
                null,
                showMessage: false,
                allowAnyHeroPower: true,
                allowReplacements: true,
                canBeCancelled: true,
                null,
                forceDecision: false,
                allowOutOfPlayPower: false,
                FindCardController(repOfEarth).GetCardSource()
            );
            RunCoroutine(e);
        }
    }
}
