using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;
using Handelabra;
using MathNet.Numerics;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Alexandria
{
    [TestFixture()]
    public class AlexandriaTests : ParahumanTest
    {
        [Test()]
        public void TestCantReturnOneshotsOrYourCharacterCardOrNotYourCards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "CaptainCosmic", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            DecisionSelectLocation = new LocationChoice(alexandria.CharacterCard.NextToLocation);

            var siphon = PlayCard("DynamicSiphon");
            var blade = PlayCard("AutonomousBlade");
            var cape1 = PlayCard("AlexandriasCape", 0);
            var cape2 = PlayCard("AlexandriasCape", 1);

            ResetDecisions();

            MoveAllCardsFromHandToDeck(alexandria);
            var strength = PutInHand("PureStrength");

            DecisionSelectCards = new Card[] { baron.CharacterCard, siphon, cape1 };
            GameControllerDecisionEvent decider = (IDecision decision) =>
            {
                if (decision is SelectCardDecision scd && scd.SelectionType == SelectionType.ReturnToHand)
                {
                    Assert.That(scd.Choices, Does.Not.Contain(strength));
                    Assert.That(scd.Choices, Does.Not.Contain(alexandria.CharacterCard));
                    Assert.That(scd.Choices, Does.Not.Contain(siphon));
                    Assert.That(scd.Choices, Does.Not.Contain(blade));

                    Assert.That(scd.Choices, Does.Contain(cape1));
                    Assert.That(scd.Choices, Does.Contain(cape2));
                }
                return MakeDecisions(decision);
            };

            ReplaceOnMakeDecisions(decider);

            PlayCard(strength);

            RestoreOnMakeDecisions(decider);

            AssertInHand(cape1);
            AssertIsInPlay(cape2);
        }

        [Test()]
        public void TestCantReturnIndestructible()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "TimeCataclysm");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            var cape1 = PlayCard("AlexandriasCape", 0);
            var cape2 = PlayCard("AlexandriasCape", 1);

            PlayCard("FixedPoint");

            MoveAllCardsFromHandToDeck(alexandria);

            UsePower(alexandria.CharacterCard);

            AssertIsInPlay(cape1);
            AssertIsInPlay(cape2);
        }
    }
}
