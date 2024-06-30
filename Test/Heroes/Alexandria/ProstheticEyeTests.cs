using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Alexandria
{
    [TestFixture()]
    public class ProstheticEyeTests : ParahumanTest
    {
        [Test()]
        public void TestWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            MoveAllCardsFromHandToDeck(alexandria);

            PlayCard("ProstheticEye");

            StackDeck("HastenDoom", "LivingForceField", "PoweredRemoteTurret");
            var topCards = baron.TurnTaker.Deck.GetTopCards(3);

            GoToDrawCardPhase(alexandria);
            AssertNextRevealReveals(topCards.ToList());

            DecisionSelectLocation = new LocationChoice(baron.TurnTaker.Deck);
            GoToEndOfTurn(alexandria);
        }
    }
}
