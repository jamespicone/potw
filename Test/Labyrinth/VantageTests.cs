using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Jp.ParahumansOfTheWormverse.Labyrinth;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Labyrinth
{
    [TestFixture()]
    public class VantageTests : ParahumanTest
    {
        [Test()]
        public void TestPowerNoEnv()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "InsulaPrimalis");

            RemoveVillainTriggers();
            MoveAllCards(FindEnvironment(), FindEnvironment().TurnTaker.Deck, FindEnvironment().TurnTaker.OutOfGame);
            MoveAllCards(baron, baron.TurnTaker.Deck, baron.TurnTaker.OutOfGame);
            StartGame();

            QuickHandStorage(tattletale);
            DecisionSelectCardToPlay = tattletale.HeroTurnTaker.Hand.Cards.First();
            var vantage = PlayCard("Vantage");
            UsePower(vantage);
            QuickHandCheck(0);            
        }

        [Test()]
        public void TestPowerPlayOptional()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "Legacy", "InsulaPrimalis");

            RemoveVillainTriggers();
            MoveAllCards(FindEnvironment(), FindEnvironment().TurnTaker.Deck, FindEnvironment().TurnTaker.OutOfGame);
            MoveAllCards(baron, baron.TurnTaker.Deck, baron.TurnTaker.OutOfGame);
            StartGame();

            QuickHandStorage(tattletale);

            var field1 = PlayCard("ObsidianField");
            var field2 = PlayCard("ObsidianField", 1);
            DecisionSelectCards = new Card[] { field1, null };
            DecisionSelectTurnTaker = tattletale.TurnTaker;
            var vantage = PlayCard("Vantage");
            UsePower(vantage);
            QuickHandCheck(0);

            AssertInTrash(field1);
            AssertIsInPlay(field2);
        }

        [Test()]
        public void TestPowerPlaysACard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "Legacy", "InsulaPrimalis");

            RemoveVillainTriggers();
            MoveAllCards(FindEnvironment(), FindEnvironment().TurnTaker.Deck, FindEnvironment().TurnTaker.OutOfGame);
            MoveAllCards(baron, baron.TurnTaker.Deck, baron.TurnTaker.OutOfGame);
            StartGame();

            var field1 = PlayCard("ObsidianField");
            var field2 = PlayCard("ObsidianField", 1);
            var confidence = PutInHand("Confidence");
            DecisionSelectCards = new Card[] { field1, confidence };
            DecisionSelectTurnTaker = tattletale.TurnTaker;
            var vantage = PlayCard("Vantage");

            QuickHandStorage(tattletale);
            UsePower(vantage);
            QuickHandCheck(-1);

            AssertInTrash(field1);
            AssertIsInPlay(field2);
            AssertIsInPlay(confidence);
        }

        [Test()]
        public void TestGivesAPower()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "Legacy", "InsulaPrimalis");

            RemoveVillainTriggers();
            MoveAllCards(FindEnvironment(), FindEnvironment().TurnTaker.Deck, FindEnvironment().TurnTaker.OutOfGame);
            MoveAllCards(baron, baron.TurnTaker.Deck, baron.TurnTaker.OutOfGame);
            StartGame();

            var vantage = PlayCard("Vantage");
            DecisionYesNo = true;

            GoToStartOfTurn(tattletale);
            AssertNotUsablePower(tattletale, tattletale.CharacterCard);
        }
    }
}
