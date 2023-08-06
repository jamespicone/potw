using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Echidna
{
    [TestFixture()]
    public class ExperimentationTests : ParahumanTest
    {
        [Test()]
        public void TestSayingNo()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "InsulaPrimalis"
            );

            StartGame();
            ReturnAllTwisted();

            StackDeck("ResistanceTwisted");
            StackDeck("PandemicTwisted");
            StackDeck("HubrisTwisted");

            PutInTrash(echidna.TurnTaker.Deck.Cards);

            DecisionYesNo = false;
            PlayCard("Experimentation");

            AssertIsInPlay("HubrisTwisted");
            AssertIsInPlay("PandemicTwisted");
            AssertNotInPlay("ResistanceTwisted");

            AssertNotFlipped(echidna.CharacterCard);

            AssertNumberOfCardsInTrash(echidna, 1);
        }

        [Test()]
        public void TestAlreadyFlipped()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "InsulaPrimalis"
            );

            StartGame();
            ReturnAllTwisted();

            StackDeck("ResistanceTwisted");
            StackDeck("PandemicTwisted");
            StackDeck("HubrisTwisted");

            PutInTrash(echidna.TurnTaker.Deck.Cards);
            FlipCard(echidna.CharacterCard);

            DecisionYesNo = true;
            PlayCard("Experimentation");

            AssertIsInPlay("HubrisTwisted");
            AssertIsInPlay("PandemicTwisted");
            AssertNotInPlay("ResistanceTwisted");

            AssertFlipped(echidna.CharacterCard);

            AssertNumberOfCardsInTrash(echidna, 1);
        }

        [Test()]
        public void TestSayingYes()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "InsulaPrimalis"
            );

            StartGame();
            ReturnAllTwisted();

            StackDeck("ResistanceTwisted");
            StackDeck("PandemicTwisted");
            StackDeck("HubrisTwisted");

            PutInTrash(echidna.TurnTaker.Deck.Cards);

            DecisionYesNo = true;
            PlayCard("Experimentation");

            AssertNotInPlay("HubrisTwisted");
            AssertNotInPlay("PandemicTwisted");
            AssertNotInPlay("ResistanceTwisted");

            AssertFlipped(echidna.CharacterCard);

            AssertNumberOfCardsInTrash(echidna, 1);
        }
    }
}
