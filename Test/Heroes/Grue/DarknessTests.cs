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
    public class DarknessTests : ParahumanTest
    {
        [Test()]
        public void TestFallsOffWhenTargetDestroyed()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "InsulaPrimalis");

            StartGame();

            var platform = GetMobileDefensePlatform();
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(platform.Card));

            AssertIsInPlay(platform.Card);
            AssertNumberOfCardsNextToCard(platform.Card, 1);

            var darkness = platform.Card.GetAllNextToCards(false).FirstOrDefault();
            Assert.That(darkness.Identifier, Is.EqualTo("Darkness"));

            DestroyCard(platform.Card);

            AssertInTrash(platform.Card);

            AssertNumberOfCardsNextToCard(platform.Card, 0);

            AssertIsInPlay(darkness);
        }

        [Test()]
        public void TestLeavesGameIfDestroyed()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "InsulaPrimalis");

            StartGame();

            RemoveVillainTriggers();

            var platform = GetMobileDefensePlatform();
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(platform.Card));

            AssertIsInPlay(platform.Card);
            AssertNumberOfCardsNextToCard(platform.Card, 1);

            var darkness = platform.Card.GetAllNextToCards(false).FirstOrDefault();
            Assert.That(darkness.Identifier, Is.EqualTo("Darkness"));

            DestroyCard(darkness);

            AssertIsInPlay(platform.Card);

            AssertNumberOfCardsNextToCard(platform.Card, 0);

            AssertOutOfGame(darkness);
        }

        [Test()]
        public void TestLeavesGameAfterATurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "InsulaPrimalis");

            StartGame();

            RemoveVillainTriggers();

            GoToUsePowerPhase(grue);

            var platform = GetMobileDefensePlatform();
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(platform.Card));

            AssertIsInPlay(platform.Card);
            AssertNumberOfCardsNextToCard(platform.Card, 1);

            var darkness = platform.Card.GetAllNextToCards(false).FirstOrDefault();
            Assert.That(darkness.Identifier, Is.EqualTo("Darkness"));

            GoToStartOfTurn(grue);

            AssertIsInPlay(platform.Card);
            AssertNumberOfCardsNextToCard(platform.Card, 1);
            AssertIsInPlay(darkness);

            GoToDrawCardPhase(grue);

            AssertIsInPlay(platform.Card);
            AssertNumberOfCardsNextToCard(platform.Card, 1);
            AssertIsInPlay(darkness);

            GoToEndOfTurn(grue);

            AssertOutOfGame(darkness);
        }
    }
}
