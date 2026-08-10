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

        // Every card that can use a power on a Representative of Earth summon (Called to
        // Judgement, Character Witness, Guise's "I Can Do That Too!" and Completionist Guise -
        // the only four with allowAnyHeroPower) replaces the turn taker controller for the
        // duration, so "you" is the borrowing hero: their deck, their hand, their character card.
        [Test()]
        public void TestPowerLentByCalledToJudgement()
        {
            SetupGameController("BaronBlade", "Legacy", "Bunker", "TheCelestialTribunal");
            StartGame();

            DecisionSelectFromBoxIdentifiers = new string[] { "Jp.ParahumansOfTheWormverse.BatteryCharacter" };
            DecisionSelectFromBoxTurnTakerIdentifier = "Jp.ParahumansOfTheWormverse.Battery";
            PlayCard("RepresentativeOfEarth");
            ResetDecisions();

            // Legacy is chosen to use the power, so DecisionSelectPower is his card - inside the
            // window the summoned controller reports Legacy's character card as its own.
            QuickHandStorage(legacy);
            DecisionSelectCard = legacy.CharacterCard;
            DecisionSelectPower = legacy.CharacterCard;
            PlayCard("CalledToJudgement");

            // "{Charge} {BatteryCharacter} and draw a card" drew from Legacy's deck...
            QuickHandCheck(1);

            // ...and charged Legacy, not the summoned card.
            var effect = GameController.StatusEffectControllers
                .Select(sc => sc.StatusEffect)
                .OfType<BatteryChargedStatusEffect>()
                .Single();
            Assert.That(effect.ChargedCard, Is.EqualTo(legacy.CharacterCard));
        }

        // Using a power on the summoned card with no replacement in place is not something the
        // base game can produce, but it is how the sweep for these crashes drives them and it is
        // the state the null CharacterCard lives in, so keep it guarded.
        [Test()]
        public void TestBroughtInByRepresentativeOfEarth()
        {
            SetupGameController("BaronBlade", "Legacy", "TheCelestialTribunal");
            StartGame();

            DecisionSelectFromBoxIdentifiers = new string[] { "Jp.ParahumansOfTheWormverse.BatteryCharacter" };
            DecisionSelectFromBoxTurnTakerIdentifier = "Jp.ParahumansOfTheWormverse.Battery";
            PlayCard("RepresentativeOfEarth");
            ResetDecisions();

            var batteryCard = GameController.FindCardsWhere(
                c => c.IsInPlayAndHasGameText && c.IsHeroCharacterCard && c.Owner.IsEnvironment,
                realCardsOnly: false).FirstOrDefault();
            Assert.That(batteryCard, Is.Not.Null, "Representative of Earth did not bring in Battery");

            var batteryController = FindCardController(batteryCard);
            Assert.That(batteryController.IsCharged(batteryCard), Is.False);

            // "{Charge} {BatteryCharacter} and draw a card" - there is no deck to draw from.
            UsePower(batteryCard, 0);

            Assert.That(batteryController.IsCharged(batteryCard), Is.True);
            AssertNumberOfStatusEffectsInPlay(1);

            var effect = GameController.StatusEffectControllers
                .Select(sc => sc.StatusEffect)
                .OfType<BatteryChargedStatusEffect>()
                .Single();
            Assert.That(effect.ChargedCard, Is.EqualTo(batteryCard));

            // Now charged, the only contributed power is "{Discharge} {BatteryCharacter} and you
            // may play a card" - there is no hand to play from. Index 0 because the card has no
            // printed powers of its own, so the contributed one is the whole list.
            UsePower(batteryCard, 0);

            Assert.That(batteryController.IsCharged(batteryCard), Is.False);
            AssertNumberOfStatusEffectsInPlay(0);
        }
    }
}
