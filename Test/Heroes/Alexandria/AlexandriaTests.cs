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

        // The Celestial Tribunal's Representative of Earth puts our character card into play
        // owned by the environment: no HeroTurnTakerController, no CharacterCard, and no deck,
        // hand or trash.
        [Test()]
        public void TestBroughtInByRepresentativeOfEarth()
        {
            SetupGameController("BaronBlade", "Legacy", "TheCelestialTribunal");
            StartGame();

            var alexandriaCard = SummonRepresentativeOfEarth("Alexandria", "AlexandriaCharacter");

            GoToStartOfTurn(legacy);
            GoToStartOfTurn(env);
            GoToStartOfTurn(baron);
            AssertIsInPlay(alexandriaCard);

            // "You may play a card" has no hand to play from and "return one of your noncharacter
            // cards in play to your hand" has no cards of ours in play, so both no-op.
            UsePower(alexandriaCard, 0);

            AssertIsInPlay(alexandriaCard);
        }

        [Test()]
        public void TestPowerLentByCalledToJudgement()
        {
            SetupGameController("BaronBlade", "Legacy", "TheCelestialTribunal");
            StartGame();

            SummonRepresentativeOfEarth("Alexandria", "AlexandriaCharacter");

            var inPlay = PlayCard("MotivationalCharge");
            var inHand = PutInHand("Fortitude");
            DecisionSelectCards = new Card[] { inHand, inPlay };

            UsePowerLentByCalledToJudgement(legacy.CharacterCard);

            // Both halves used Legacy's hand and play area, not the environment's.
            AssertIsInPlay(inHand);
            AssertInHand(inPlay);
        }
    }
}
