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
    public class CoverTheGetawayTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "InsulaPrimalis");

            StartGame();
        }

        [Test()]
        public void TestPlacesDarknessNextToEachHero()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "Legacy", "InsulaPrimalis");

            StartGame();

            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            PlayCard("CoverTheGetaway");

            // Grue should have Darkness
            var darknessNextToGrue = grue.CharacterCard.GetAllNextToCards(false).Where(c => c.Identifier == "Darkness");
            Assert.That(darknessNextToGrue.Count(), Is.EqualTo(1), "Should have 1 Darkness next to Grue");

            // Bunker should have Darkness
            var darknessNextToBunker = bunker.CharacterCard.GetAllNextToCards(false).Where(c => c.Identifier == "Darkness");
            Assert.That(darknessNextToBunker.Count(), Is.EqualTo(1), "Should have 1 Darkness next to Bunker");

            // Legacy should have Darkness
            var darknessNextToLegacy = legacy.CharacterCard.GetAllNextToCards(false).Where(c => c.Identifier == "Darkness");
            Assert.That(darknessNextToLegacy.Count(), Is.EqualTo(1), "Should have 1 Darkness next to Legacy");
        }

        [Test()]
        public void TestMayDestroyEnvironmentCard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            // Go to Grue's play phase to avoid any villain/env turn effects
            GoToPlayCardPhase(grue);

            var envCard = PlayCard("VelociraptorPack");
            AssertIsInPlay(envCard);

            // CoverTheGetaway places Darkness next to hero characters (allowAutoDecide=true)
            // then optionally destroys an environment card.
            // The first hero selection prompts, then remaining heroes auto-select,
            // then the destruction decision prompts.
            DecisionSelectCards = new Card[] { grue.CharacterCard, envCard };

            PlayCard("CoverTheGetaway");

            AssertInTrash(envCard);
        }

        [Test()]
        public void TestEnvironmentDestructionOptional()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var envCard = PlayCard("VelociraptorPack");
            AssertIsInPlay(envCard);

            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            PlayCard("CoverTheGetaway");

            // Environment card should still be in play
            AssertIsInPlay(envCard);
        }

        [Test()]
        public void TestDoesNotPlaceOnVillains()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            PlayCard("CoverTheGetaway");

            // Baron should NOT have Darkness
            var darknessNextToBaron = baron.CharacterCard.GetAllNextToCards(false).Where(c => c.Identifier == "Darkness");
            Assert.That(darknessNextToBaron.Count(), Is.EqualTo(0), "Baron should not have Darkness");
        }

        [Test()]
        public void TestIsOneShot()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var card = GetCard("CoverTheGetaway");

            Assert.That(card.DoKeywordsContain("one-shot"), Is.True, "Cover the Getaway should be a one-shot");
        }

        [Test()]
        public void TestTargetsHeroCharacterCards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            // Play a hero target that is NOT a character card
            var turret = PlayCard("OmniCannon");

            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            PlayCard("CoverTheGetaway");

            // Turret should NOT have Darkness (it's not a character card)
            var darknessNextToTurret = turret.GetAllNextToCards(false).Where(c => c.Identifier == "Darkness");
            Assert.That(darknessNextToTurret.Count(), Is.EqualTo(0), "Turret should not have Darkness");
        }
    }
}
