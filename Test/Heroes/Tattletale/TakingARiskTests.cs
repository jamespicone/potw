using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Tattletale
{
    [TestFixture()]
    public class TakingARiskTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();
        }

        [Test()]
        public void TestIsOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            var card = GetCard("TakingARisk");
            Assert.That(card.DoKeywordsContain("ongoing"), Is.True);
        }

        [Test()]
        public void TestPowerPutsTopCardIntoPlay()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            var risk = PlayCard("TakingARisk");

            // Put an ongoing on top of Tattletale's deck
            var confidence = PutOnDeck("Confidence");

            DecisionSelectLocation = new LocationChoice(tattletale.TurnTaker.Deck);
            UsePower(risk);

            AssertIsInPlay(confidence);
        }

        [Test()]
        public void TestCanTargetVillainDeck()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            var risk = PlayCard("TakingARisk");

            // Stack a card that stays in play (ongoing/equipment/target) on top of villain deck
            var livingForceField = StackDeck("LivingForceField");

            DecisionSelectLocation = new LocationChoice(baron.TurnTaker.Deck);
            UsePower(risk);

            AssertIsInPlay(livingForceField);
        }
    }
}
