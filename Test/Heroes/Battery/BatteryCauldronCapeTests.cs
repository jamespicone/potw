using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Jp.ParahumansOfTheWormverse.Battery;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Battery
{
    [TestFixture()]
    public class BatteryCauldronCapeTests : ParahumanTest
    {
        [Test()]
        public void TestDischargeAtEndOfTurn()
        {
            SetupGameController(
                new string[] { "BaronBlade", "Jp.ParahumansOfTheWormverse.Battery", "Megalopolis" },
                promoIdentifiers: new Dictionary<string, string> { { "Jp.ParahumansOfTheWormverse.Battery", "BatteryCauldronCapeCharacter" } }
            );

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            GoToUsePowerPhase(battery);

            Assert.That(battery.CharacterCardController.IsCharged(battery.CharacterCard), Is.False);

            AssertNumberOfStatusEffectsInPlay(0);
            UsePower(battery, 1);
            AssertNumberOfStatusEffectsInPlay(1);
            Assert.That(battery.CharacterCardController.IsCharged(battery.CharacterCard), Is.True);

            GoToEndOfTurn(battery);

            AssertNumberOfStatusEffectsInPlay(1);
            Assert.That(battery.CharacterCardController.IsCharged(battery.CharacterCard), Is.True);

            GoToStartOfTurn(battery);

            Assert.That(battery.CharacterCardController.IsCharged(battery.CharacterCard), Is.False);
            AssertNumberOfStatusEffectsInPlay(0);
        }

        [Test()]
        public void TestFaceDownACard()
        {
            SetupGameController(
                new string[] { "BaronBlade", "Jp.ParahumansOfTheWormverse.Battery", "Megalopolis" },
                promoIdentifiers: new Dictionary<string, string> { { "Jp.ParahumansOfTheWormverse.Battery", "BatteryCauldronCapeCharacter" } }
            );

            StartGame();

            MoveAllCardsFromHandToDeck(battery);

            var cards = StackDeck("CoolToys", "Strength", "RapidRecon");

            var coolToys = cards.ElementAt(0);
            var strength = cards.ElementAt(1);
            var rapidRecon = cards.ElementAt(2);

            AssertInDeck(coolToys);
            AssertInDeck(strength);
            AssertInDeck(rapidRecon);

            UsePower(battery, 0);

            AssertInDeck(coolToys);
            AssertInDeck(strength);
            AssertInPlayArea(battery, rapidRecon);
            AssertFlipped(rapidRecon);

            UsePower(battery, 0);

            AssertInDeck(coolToys);
            AssertInPlayArea(battery, strength);
            AssertFlipped(strength);
            AssertInPlayArea(battery, rapidRecon);
            AssertFlipped(rapidRecon);

            UsePower(battery, 0);

            AssertInPlayArea(battery, coolToys);
            AssertFlipped(coolToys);
            AssertInPlayArea(battery, strength);
            AssertFlipped(strength);
            AssertInPlayArea(battery, rapidRecon);
            AssertFlipped(rapidRecon);
        }

        [Test()]
        public void TestPlayACard()
        {
            SetupGameController(
                new string[] { "BaronBlade", "Jp.ParahumansOfTheWormverse.Battery", "Megalopolis" },
                promoIdentifiers: new Dictionary<string, string> { { "Jp.ParahumansOfTheWormverse.Battery", "BatteryCauldronCapeCharacter" } }
            );

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            MoveAllCardsFromHandToDeck(battery);

            StackDeck("Strength");

            UsePower(battery, 0);

            GoToUsePowerPhase(battery);

            DecisionSelectTarget = baron.CharacterCard;
            QuickHPStorage(baron);
            UsePower(battery, 1);
            QuickHPCheck(-5);

            Assert.That(battery.CharacterCardController.IsCharged(battery.CharacterCard), Is.True);
            AssertNumberOfStatusEffectsInPlay(1);

            GoToStartOfTurn(battery);

            Assert.That(battery.CharacterCardController.IsCharged(battery.CharacterCard), Is.False);
            AssertNumberOfStatusEffectsInPlay(0);
        }

        [Test()]
        public void TestOnlyLimitedAndNoPlays()
        {
            SetupGameController(
                new string[] { "BaronBlade", "Jp.ParahumansOfTheWormverse.Battery", "Megalopolis" },
                promoIdentifiers: new Dictionary<string, string> { { "Jp.ParahumansOfTheWormverse.Battery", "BatteryCauldronCapeCharacter" } }
            );

            StartGame();

            DestroyNonCharacterVillainCards();
            MoveAllCardsFromHandToDeck(battery);

            var threadsInPlay = PlayCard("GlowingThreads");
            var threadsOnDeck = StackDeck("GlowingThreads");

            UsePower(battery, 0);

            AssertFlipped(threadsOnDeck);
            AssertInPlayArea(battery, threadsOnDeck);
            AssertInPlayArea(battery, threadsInPlay);

            UsePower(battery, 1);

            AssertInPlayArea(battery, threadsInPlay);
            AssertNotFlipped(threadsOnDeck);
            AssertInTrash(threadsOnDeck);
        }

        [Test()]
        public void TestLimitedBecomesUnplayable()
        {
            SetupGameController(
                new string[] { "BaronBlade", "Jp.ParahumansOfTheWormverse.Battery", "Megalopolis" },
                promoIdentifiers: new Dictionary<string, string> { { "Jp.ParahumansOfTheWormverse.Battery", "BatteryCauldronCapeCharacter" } }
            );

            StartGame();

            DestroyNonCharacterVillainCards();
            MoveAllCardsFromHandToDeck(battery);

            var threadsOnDeck = FindCardsWhere(c => c.Identifier == "GlowingThreads");
            StackDeck(battery, threadsOnDeck);
            var firstCard = threadsOnDeck.ElementAt(0);
            var secondCard = threadsOnDeck.ElementAt(1);

            UsePower(battery, 0);
            UsePower(battery, 0);

            AssertFlipped(firstCard);
            AssertFlipped(secondCard);
            AssertInPlayArea(battery, firstCard);
            AssertInPlayArea(battery, secondCard);

            DecisionSelectCards = new Card[] { firstCard, secondCard, null };
            UsePower(battery, 1);

            AssertNotFlipped(firstCard);
            AssertNotFlipped(secondCard);

            AssertIsInPlay(firstCard);
            AssertInTrash(secondCard);
        }

        [Test()]
        public void TestSelectingUnplayableLimited()
        {
            SetupGameController(
                new string[] { "BaronBlade", "Jp.ParahumansOfTheWormverse.Battery", "Megalopolis" },
                promoIdentifiers: new Dictionary<string, string> { { "Jp.ParahumansOfTheWormverse.Battery", "BatteryCauldronCapeCharacter" } }
            );

            StartGame();

            DestroyNonCharacterVillainCards();
            MoveAllCardsFromHandToDeck(battery);

            var threadsInPlay = PlayCard("GlowingThreads");
            var stack = StackDeck("GlowingThreads", "Strength");

            var threadsOnDeck = stack.ElementAt(0);
            var strength = stack.ElementAt(1);

            UsePower(battery, 0);
            UsePower(battery, 0);

            AssertFlipped(threadsOnDeck);
            AssertInPlayArea(battery, threadsOnDeck);
            AssertFlipped(strength);
            AssertInPlayArea(battery, strength);

            DecisionSelectCards = new Card[] { threadsOnDeck, baron.CharacterCard, null };
            QuickHPStorage(baron);
            UsePower(battery, 1);

            AssertInPlayArea(battery, threadsInPlay);
            AssertNotFlipped(threadsOnDeck);
            AssertInTrash(threadsOnDeck);
            QuickHPCheck(-5);
        }

        [Test()]
        public void TestSwitchingCharacterCardsDoesntUncharge()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Battery/BatteryCauldronCapeCharacter", "Guise/CompletionistGuiseCharacter", "InsulaPrimalis");

            StartGame();

            UsePower(battery.CharacterCard, 1);
            Assert.That(battery.CharacterCardController.IsCharged(battery.CharacterCard), Is.True);

            DecisionSelectCards = new Card[] { battery.CharacterCard, battery.CharacterCard };
            UsePower(guise);

            Assert.That(battery.CharacterCardController.IsCharged(battery.CharacterCard), Is.True);
            Assert.That(battery.CharacterCard.PromoIdentifierOrIdentifier, Is.EqualTo("BatteryCharacter"));
        }

        // Every card that can use a power on a Representative of Earth summon (Called to
        // Judgement, Character Witness, Guise's "I Can Do That Too!" and Completionist Guise -
        // the only four with allowAnyHeroPower) replaces the turn taker controller for the
        // duration, so "your deck" and "your play area" are the borrowing hero's.
        [Test()]
        public void TestPowerLentByCalledToJudgement()
        {
            SetupGameController("BaronBlade", "Legacy", "Bunker", "TheCelestialTribunal");
            StartGame();

            DecisionSelectFromBoxIdentifiers = new string[] { "Jp.ParahumansOfTheWormverse.BatteryCauldronCapeCharacter" };
            DecisionSelectFromBoxTurnTakerIdentifier = "Jp.ParahumansOfTheWormverse.Battery";
            PlayCard("RepresentativeOfEarth");
            ResetDecisions();

            // An ongoing, so it is still in play to look at once the discharge plays it.
            var legacyTop = PutOnDeck("InspiringPresence");

            // Legacy is chosen to use the power, so DecisionSelectPower is his card - inside the
            // window the summoned controller reports Legacy's character card as its own.
            DecisionSelectCard = legacy.CharacterCard;
            DecisionSelectPower = legacy.CharacterCard;
            DecisionSelectPowerIndex = 0;
            PlayCard("CalledToJudgement");

            // "Put the top card of your deck into play face down" used Legacy's deck and play area.
            AssertInPlayArea(legacy, legacyTop);
            AssertFlipped(legacyTop);

            // And the charge power charges Legacy, not the summoned card.
            DecisionSelectCard = legacy.CharacterCard;
            DecisionSelectPower = legacy.CharacterCard;
            DecisionSelectPowerIndex = 1;
            PlayCard("CalledToJudgement");

            var effect = GameController.StatusEffectControllers
                .Select(sc => sc.StatusEffect)
                .OfType<BatteryChargedStatusEffect>()
                .Single();
            Assert.That(effect.ChargedCard, Is.EqualTo(legacy.CharacterCard));

            // The face-down card was revealed and put into play, again from Legacy's play area.
            AssertNotFlipped(legacyTop);
            AssertIsInPlay(legacyTop);

            // "{Charge} {BatteryCharacter} until the start of your next turn" expires off the
            // borrowing hero's turn too - ChargeCard hangs UntilStartOfNextTurn on TurnTaker,
            // which follows the replacement to Legacy, not the summoned card's environment.
            GoToEndOfTurn(baron);
            Assert.That(legacy.CharacterCardController.IsCharged(legacy.CharacterCard), Is.True);
            AssertNumberOfStatusEffectsInPlay(1);

            GoToStartOfTurn(legacy);

            Assert.That(legacy.CharacterCardController.IsCharged(legacy.CharacterCard), Is.False);
            AssertNumberOfStatusEffectsInPlay(0);
        }

        // Using a power on the summoned card with no replacement in place is not something the
        // base game can produce, but it is how the sweep for these crashes drives them and it is
        // the state the null CharacterCard lives in, so keep it guarded.
        [Test()]
        public void TestBroughtInByRepresentativeOfEarth()
        {
            SetupGameController("BaronBlade", "Legacy", "TheCelestialTribunal");
            StartGame();

            DecisionSelectFromBoxIdentifiers = new string[] { "Jp.ParahumansOfTheWormverse.BatteryCauldronCapeCharacter" };
            DecisionSelectFromBoxTurnTakerIdentifier = "Jp.ParahumansOfTheWormverse.Battery";
            PlayCard("RepresentativeOfEarth");
            ResetDecisions();

            var cape = GameController.FindCardsWhere(
                c => c.IsInPlayAndHasGameText && c.IsHeroCharacterCard && c.Owner.IsEnvironment,
                realCardsOnly: false).FirstOrDefault();
            Assert.That(cape, Is.Not.Null, "Representative of Earth did not bring in the promo character card");
            Assert.That(cape.Title, Is.EqualTo("Battery: Cauldron Cape"));

            var capeController = FindCardController(cape);
            Assert.That(capeController.IsCharged(cape), Is.False);

            // "Your" deck and play area are the environment's here, so the powers work on those.
            var adjudicator = PutOnDeck("CelestialAdjudicator");

            UsePower(cape, 0);

            AssertInPlayArea(env, adjudicator);
            AssertFlipped(adjudicator);

            UsePower(cape, 1);

            Assert.That(capeController.IsCharged(cape), Is.True);
            AssertNumberOfStatusEffectsInPlay(1);
            AssertNotFlipped(adjudicator);
            AssertIsInPlay(adjudicator);

            // It charges the card in play, not the hero's absent character card.
            var effect = GameController.StatusEffectControllers
                .Select(sc => sc.StatusEffect)
                .OfType<BatteryChargedStatusEffect>()
                .Single();
            Assert.That(effect.ChargedCard, Is.EqualTo(cape));

            // "Until the start of your next turn" is the environment's turn here, since that is
            // who owns him - so it survives the intervening hero turn and expires on the
            // Tribunal's. Without a turn taker that ever takes a turn this would never come due,
            // the way Grue's Darkness cards don't.
            GoToStartOfTurn(legacy);
            Assert.That(capeController.IsCharged(cape), Is.True);
            AssertNumberOfStatusEffectsInPlay(1);

            GoToStartOfTurn(env);

            Assert.That(capeController.IsCharged(cape), Is.False);
            AssertNumberOfStatusEffectsInPlay(0);
        }
    }
}
