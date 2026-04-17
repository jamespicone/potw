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
    public class HideAndRecoverTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "InsulaPrimalis");

            StartGame();
        }

        [Test()]
        public void TestPlacesDarknessNextToHero()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            DecisionSelectCard = bunker.CharacterCard;
            DecisionYesNo = true;

            PlayCard("HideAndRecover");

            var darknessNextToBunker = bunker.CharacterCard.GetAllNextToCards(false).Where(c => c.Identifier == "Darkness");
            Assert.That(darknessNextToBunker.Count(), Is.EqualTo(1), "Should have 1 Darkness next to Bunker");
        }

        [Test()]
        public void TestHeroRegains2HP()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            SetHitPoints(bunker, 20);

            DecisionSelectCard = bunker.CharacterCard;
            DecisionYesNo = true;

            QuickHPStorage(bunker);
            PlayCard("HideAndRecover");
            QuickHPCheck(2);
        }

        [Test()]
        public void TestPlayerMayDraw2Cards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            DecisionSelectCard = bunker.CharacterCard;
            DecisionYesNo = true;

            var handBefore = GetNumberOfCardsInHand(bunker);

            PlayCard("HideAndRecover");

            var handAfter = GetNumberOfCardsInHand(bunker);
            Assert.That(handAfter, Is.EqualTo(handBefore + 2));
        }

        [Test()]
        public void TestDrawsExactly2Cards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            DecisionSelectCard = bunker.CharacterCard;

            var handBefore = GetNumberOfCardsInHand(bunker);

            PlayCard("HideAndRecover");

            var handAfter = GetNumberOfCardsInHand(bunker);
            // Verify exactly 2 cards were drawn
            Assert.That(handAfter, Is.EqualTo(handBefore + 2));
        }

        [Test()]
        public void TestCanTargetAnyHeroCharacter()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            SetHitPoints(grue, 20);

            DecisionSelectCard = grue.CharacterCard;
            DecisionYesNo = true;

            QuickHPStorage(grue);
            PlayCard("HideAndRecover");
            QuickHPCheck(2);

            var darknessNextToGrue = grue.CharacterCard.GetAllNextToCards(false).Where(c => c.Identifier == "Darkness");
            Assert.That(darknessNextToGrue.Count(), Is.EqualTo(1), "Should have 1 Darkness next to Grue");
        }

        [Test()]
        public void TestIsOneShot()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var card = GetCard("HideAndRecover");

            Assert.That(card.DoKeywordsContain("one-shot"), Is.True, "Hide and Recover should be a one-shot");
        }

        [Test()]
        public void TestCannotTargetVillain()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            // Baron should not be a valid target
            AssertNextDecisionChoices(
                included: new Card[] { grue.CharacterCard, bunker.CharacterCard },
                notIncluded: new Card[] { baron.CharacterCard }
            );

            DecisionYesNo = false;
            PlayCard("HideAndRecover");
        }
    }
}
