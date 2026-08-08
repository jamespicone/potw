using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Bitch
{
    [TestFixture()]
    public class WhistleTests : ParahumanTest
    {
        [Test()]
        public void TestMayDrawAtStartOfTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "InsulaPrimalis");
            StartGame();
            RemoveVillainCards();
            RemoveVillainTriggers();

            PlayCard("Whistle");

            QuickHandStorage(bitch);
            DecisionYesNo = true;

            GoToStartOfTurn(bitch);

            QuickHandCheck(1);
        }

        [Test()]
        public void TestMayPlayOrderAtEndOfTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "InsulaPrimalis");
            StartGame();
            RemoveVillainCards();
            RemoveVillainTriggers();

            PlayCard("Whistle");

            var heel = PutInHand("Heel");

            DecisionSelectCard = heel;
            DecisionSelectLocation = new LocationChoice(bitch.TurnTaker.Deck);
            DecisionYesNo = false; // decline the optional draws

            GoToEndOfTurn(bitch);

            // Heel was played from hand at the end of the turn, fetching a dog.
            // (Which dog is revealed depends on the draw phase, so count them.)
            AssertInTrash(heel);
            Assert.That(
                FindCardsWhere(c => c.DoKeywordsContain("dog") && c.IsInPlay).Count(),
                Is.EqualTo(1));
        }
    }
}
