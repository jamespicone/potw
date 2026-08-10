using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;
using Jp.ParahumansOfTheWormverse.Grue;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Grue
{
    [TestFixture()]
    public class GrueTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "InsulaPrimalis");

            StartGame();

            Assert.That(grue, Is.Not.Null);
        }

        [Test()]
        public void TestHas30HP()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "InsulaPrimalis");

            StartGame();

            Assert.That(grue.CharacterCard.MaximumHitPoints, Is.EqualTo(30));
        }

        [Test()]
        public void TestPower_PlacesDarknessNextToGrue()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            DecisionSelectCard = baron.CharacterCard;

            UsePower(grue.CharacterCard);

            // Check that Darkness is next to Grue
            var darknessNextToGrue = grue.CharacterCard.GetAllNextToCards(false).Where(c => c.Identifier == "Darkness");
            Assert.That(darknessNextToGrue.Count(), Is.EqualTo(1), "Should have 1 Darkness next to Grue");
        }

        [Test()]
        public void TestPower_PlacesDarknessNextToOtherTarget()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            DecisionSelectCard = baron.CharacterCard;

            UsePower(grue.CharacterCard);

            // Check that Darkness is next to Baron
            var darknessNextToBaron = baron.CharacterCard.GetAllNextToCards(false).Where(c => c.Identifier == "Darkness");
            Assert.That(darknessNextToBaron.Count(), Is.EqualTo(1), "Should have 1 Darkness next to Baron");
        }

        [Test()]
        public void TestPower_CanSelectVillainTarget()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            DecisionSelectCard = baron.CharacterCard;

            UsePower(grue.CharacterCard);

            var darknessNextToBaron = baron.CharacterCard.GetAllNextToCards(false).Where(c => c.Identifier == "Darkness");
            Assert.That(darknessNextToBaron.Count(), Is.EqualTo(1));
        }

        [Test()]
        public void TestPower_CanSelectHeroTarget()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            DecisionSelectCard = bunker.CharacterCard;

            UsePower(grue.CharacterCard);

            var darknessNextToBunker = bunker.CharacterCard.GetAllNextToCards(false).Where(c => c.Identifier == "Darkness");
            Assert.That(darknessNextToBunker.Count(), Is.EqualTo(1));
        }

        [Test()]
        public void TestPower_CanSelectEnvironmentTarget()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var envTarget = PlayCard("VelociraptorPack");

            DecisionSelectCard = envTarget;

            UsePower(grue.CharacterCard);

            var darknessNextToEnv = envTarget.GetAllNextToCards(false).Where(c => c.Identifier == "Darkness");
            Assert.That(darknessNextToEnv.Count(), Is.EqualTo(1));
        }

        [Test()]
        public void TestTriggerPower_NotAvailableByDefault()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            // The character controller should not be able to use trigger powers by default
            var canUseTrigger = grue.CharacterCardController.CanGrueUseTriggerPowers();
            Assert.That(canUseTrigger, Is.False, "Grue should not be able to use Trigger powers by default");
        }

        [Test()]
        public void TestTriggerPower_AvailableAfterSecondTrigger()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            PlayCard("SecondTrigger");

            var canUseTrigger = grue.CharacterCardController.CanGrueUseTriggerPowers();
            Assert.That(canUseTrigger, Is.True, "Grue should be able to use Trigger powers after Second Trigger");
        }

        [Test()]
        public void TestTriggerPower_SelectPlayerToUsePower()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("SecondTrigger");

            // Bunker's default power draws a card
            var bunkerHandBefore = GetNumberOfCardsInHand(bunker);

            DecisionSelectTurnTaker = bunker.TurnTaker;
            DecisionSelectTarget = baron.CharacterCard;

            // Use the Trigger power (index 1)
            UsePower(grue.CharacterCard, 1);

            var bunkerHandAfter = GetNumberOfCardsInHand(bunker);
            Assert.That(bunkerHandAfter, Is.EqualTo(bunkerHandBefore + 1), "Bunker should have drawn a card from using his power");
        }

        [Test()]
        public void TestIncap1_ReduceNextDamageToTarget()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            IncapacitateCharacter(grue.CharacterCard, baron.CharacterCard);

            DecisionSelectCard = bunker.CharacterCard;

            UseIncapacitatedAbility(grue, 0);

            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Melee);
            QuickHPCheck(-2); // 3 - 1 = 2
        }

        [Test()]
        public void TestIncap1_ReductionExpiresAfterUse()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            IncapacitateCharacter(grue.CharacterCard, baron.CharacterCard);

            DecisionSelectCard = bunker.CharacterCard;

            UseIncapacitatedAbility(grue, 0);

            // First damage is reduced
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Melee);
            QuickHPCheck(-2);

            // Second damage is not reduced
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestIncap2_Deal2MeleeDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            IncapacitateCharacter(grue.CharacterCard, baron.CharacterCard);

            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            UseIncapacitatedAbility(grue, 1);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestIncap2_DamageTypeIsMelee()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            IncapacitateCharacter(grue.CharacterCard, baron.CharacterCard);

            DecisionSelectTarget = baron.CharacterCard;

            AssertDamageType(DamageType.Melee);
            UseIncapacitatedAbility(grue, 1);
        }

        [Test()]
        public void TestIncap3_DestroyOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var ongoing = PlayCard("LivingForceField");
            AssertIsInPlay(ongoing);

            IncapacitateCharacter(grue.CharacterCard, baron.CharacterCard);

            DecisionSelectCard = ongoing;

            UseIncapacitatedAbility(grue, 2);

            AssertInTrash(ongoing);
        }

        [Test()]
        public void TestIncap3_CanTargetVillainOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var ongoing = PlayCard("LivingForceField");
            AssertIsInPlay(ongoing);

            IncapacitateCharacter(grue.CharacterCard, baron.CharacterCard);

            DecisionSelectCard = ongoing;

            UseIncapacitatedAbility(grue, 2);

            AssertInTrash(ongoing);
        }

        [Test()]
        public void TestIncap3_CanTargetHeroOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "Legacy", "InsulaPrimalis");

            StartGame();

            // Use a Legacy ongoing (not Grue's) since Grue's cards go out of game when incapacitated
            var ongoing = PlayCard("InspiringPresence");
            AssertIsInPlay(ongoing);

            IncapacitateCharacter(grue.CharacterCard, baron.CharacterCard);

            DecisionSelectCard = ongoing;

            UseIncapacitatedAbility(grue, 2);

            AssertInTrash(ongoing);
        }

        // The Celestial Tribunal's Representative of Earth brings a hero character card in from
        // the box on its own, without its deck, so there are no Darkness cards anywhere in the
        // game - not even one to copy a definition from. Bunker rather than Legacy so Baron
        // Blade's nemesis bonus stays out of the damage numbers.
        [Test()]
        public void TestBroughtInByRepresentativeOfEarthWithNoDeck()
        {
            SetupGameController("BaronBlade", "Bunker", "TheCelestialTribunal");
            StartGame();

            DecisionSelectFromBoxIdentifiers = new string[] { "Jp.ParahumansOfTheWormverse.GrueCharacter" };
            DecisionSelectFromBoxTurnTakerIdentifier = "Jp.ParahumansOfTheWormverse.Grue";
            PlayCard("RepresentativeOfEarth");
            ResetDecisions();

            var grueCard = GameController.FindCardsWhere(
                c => c.IsInPlayAndHasGameText && c.IsHeroCharacterCard && c.Owner.IsEnvironment,
                realCardsOnly: false).FirstOrDefault();
            Assert.That(grueCard, Is.Not.Null, "Representative of Earth did not bring in Grue");
            Assert.That(GameController.FindCardsWhere(c => c.IsGrueDarkness(), realCardsOnly: false), Is.Empty);

            DecisionSelectCard = bunker.CharacterCard;
            UsePower(grueCard, 0);

            // Both Darkness cards get built from the deck definition, and get real controllers
            // rather than the plain CardController the factory falls back to.
            var darknesses = GameController.FindCardsWhere(c => c.IsGrueDarkness(), realCardsOnly: false).ToList();
            Assert.That(darknesses.Count, Is.EqualTo(2));
            Assert.That(darknesses.All(d => GameController.FindCardController(d) is DarknessCardController));
            Assert.That(darknesses.Select(d => d.Location.OwnerCard),
                Is.EquivalentTo(new Card[] { grueCard, bunker.CharacterCard }));

            // Grue's own damage is never reduced, so this also leaves the once-per-turn
            // reduction unspent for the check below.
            QuickHPStorage(bunker.CharacterCard);
            DealDamage(grueCard, bunker.CharacterCard, 3, DamageType.Melee);
            QuickHPCheck(-3);

            QuickHPStorage(bunker.CharacterCard);
            DealDamage(baron, bunker, 3, DamageType.Melee);
            QuickHPCheck(-2);

            // "At the end of {GrueCharacter}'s next turn remove this card from the game" never
            // comes due, because he is not a player and never takes a turn.
            for (var i = 0; i < 3; i++)
            {
                GoToStartOfTurn(env);
                GoToEndOfTurn(env);
            }

            Assert.That(darknesses.All(d => d.IsInPlayAndHasGameText), "The Darkness cards should never leave play");
        }
    }
}
