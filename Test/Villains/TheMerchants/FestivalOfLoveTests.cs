using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.TheMerchants
{
    [TestFixture()]
    public class FestivalOfLoveTests : ParahumanTest
    {
        [Test()]
        public void TestWorks()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            PlayCard("FestivalOfLove");

            GoToEndOfTurn(env);
            AssertNumberOfCardsInPlay(c => c.DoKeywordsContain("thug"), 0);

            GoToStartOfTurn(merchants);
            AssertNumberOfCardsInPlay(c => c.DoKeywordsContain("thug"), 1);

            GoToEndOfTurn(env);
            AssertNumberOfCardsInPlay(c => c.DoKeywordsContain("thug"), 1);

            GoToStartOfTurn(merchants);
            AssertNumberOfCardsInPlay(c => c.DoKeywordsContain("thug"), 2);
        }

        [Test()]
        public void TestEmptyThugDeckDoesNothing()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            PlayCard("FestivalOfLove");

            MoveAllCards(merchants, merchants.TurnTaker.FindSubDeck("ThugDeck"), merchants.TurnTaker.OffToTheSide);

            GoToStartOfTurn(merchants);
            AssertNumberOfCardsInPlay(c => c.DoKeywordsContain("thug"), 0);
            AssertNotGameOver();
        }
    }
}
