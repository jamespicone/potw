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
    public class LabyrinthTests : ParahumanTest
    {
        [Test()]
        public void TestTurnOrderSimple()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "InsulaPrimalis");

            RemoveVillainTriggers();
            MoveAllCards(FindEnvironment(), FindEnvironment().TurnTaker.Deck, FindEnvironment().TurnTaker.OutOfGame);
            MoveAllCards(baron, baron.TurnTaker.Deck, baron.TurnTaker.OutOfGame);
            StartGame();

            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(baron.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(env.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(labyrinth.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(baron.TurnTaker));
        }

        [Test()]
        public void TestTurnOrderThreeHeroesFirst()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Legacy", "Tachyon", "InsulaPrimalis");

            RemoveVillainTriggers();
            MoveAllCards(FindEnvironment(), FindEnvironment().TurnTaker.Deck, FindEnvironment().TurnTaker.OutOfGame);
            MoveAllCards(baron, baron.TurnTaker.Deck, baron.TurnTaker.OutOfGame);
            StartGame();

            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(baron.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(legacy.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(tachyon.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(env.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(labyrinth.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(baron.TurnTaker));
        }

        [Test()]
        public void TestTurnOrderThreeHeroesMiddle()
        {
            SetupGameController("BaronBlade", "Legacy", "Jp.ParahumansOfTheWormverse.Labyrinth", "Tachyon", "InsulaPrimalis");

            RemoveVillainTriggers();
            MoveAllCards(FindEnvironment(), FindEnvironment().TurnTaker.Deck, FindEnvironment().TurnTaker.OutOfGame);
            MoveAllCards(baron, baron.TurnTaker.Deck, baron.TurnTaker.OutOfGame);
            StartGame();

            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(baron.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(legacy.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(tachyon.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(env.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(labyrinth.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(baron.TurnTaker));
        }

        [Test()]
        public void TestTurnOrderThreeHerosLast()
        {
            SetupGameController("BaronBlade", "Legacy", "Tachyon", "Jp.ParahumansOfTheWormverse.Labyrinth", "InsulaPrimalis");

            RemoveVillainTriggers();
            MoveAllCards(FindEnvironment(), FindEnvironment().TurnTaker.Deck, FindEnvironment().TurnTaker.OutOfGame);
            MoveAllCards(baron, baron.TurnTaker.Deck, baron.TurnTaker.OutOfGame);
            StartGame();

            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(baron.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(legacy.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(tachyon.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(env.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(labyrinth.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(baron.TurnTaker));
        }

        [Test()]
        public void TestTurnOrderThreeHeroesFirstDice()
        {
            SetupGameController("WagerMaster", "Jp.ParahumansOfTheWormverse.Labyrinth", "Legacy", "Tachyon", "InsulaPrimalis");

            RemoveVillainTriggers();
            MoveAllCards(FindEnvironment(), FindEnvironment().TurnTaker.Deck, FindEnvironment().TurnTaker.OutOfGame);
            MoveAllCards(wager, wager.TurnTaker.Deck, wager.TurnTaker.OutOfGame);
            StartGame();

            PlayCard("PlayingDiceWithTheCosmos");

            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(wager.TurnTaker));
            GoToNextTurn();

            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(tachyon.TurnTaker));
            GoToNextTurn();

            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(legacy.TurnTaker));
            GoToNextTurn();

            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(env.TurnTaker));
            GoToNextTurn();

            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(labyrinth.TurnTaker));
            GoToNextTurn();

            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(wager.TurnTaker));
        }

        [Test()]
        public void TestTurnOrderThreeHeroesMiddleDice()
        {
            SetupGameController("WagerMaster", "Legacy", "Jp.ParahumansOfTheWormverse.Labyrinth", "Tachyon", "InsulaPrimalis");

            RemoveVillainTriggers();
            MoveAllCards(FindEnvironment(), FindEnvironment().TurnTaker.Deck, FindEnvironment().TurnTaker.OutOfGame);
            MoveAllCards(wager, wager.TurnTaker.Deck, wager.TurnTaker.OutOfGame);
            StartGame();

            PlayCard("PlayingDiceWithTheCosmos");

            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(wager.TurnTaker));
            GoToNextTurn();

            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(tachyon.TurnTaker));
            GoToNextTurn();

            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(legacy.TurnTaker));
            GoToNextTurn();

            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(env.TurnTaker));
            GoToNextTurn();

            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(labyrinth.TurnTaker));
            GoToNextTurn();

            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(wager.TurnTaker));
        }

        [Test()]
        public void TestTurnOrderThreeHerosLastDice()
        {
            SetupGameController("WagerMaster", "Legacy", "Tachyon", "Jp.ParahumansOfTheWormverse.Labyrinth", "InsulaPrimalis");

            RemoveVillainTriggers();
            MoveAllCards(FindEnvironment(), FindEnvironment().TurnTaker.Deck, FindEnvironment().TurnTaker.OutOfGame);
            MoveAllCards(wager, wager.TurnTaker.Deck, wager.TurnTaker.OutOfGame);
            StartGame();

            PlayCard("PlayingDiceWithTheCosmos");

            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(wager.TurnTaker));
            GoToNextTurn();

            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(tachyon.TurnTaker));
            GoToNextTurn();

            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(legacy.TurnTaker));
            GoToNextTurn();

            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(env.TurnTaker));
            GoToNextTurn();

            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(labyrinth.TurnTaker));
            GoToNextTurn();

            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(wager.TurnTaker));
        }

        [Test()]
        public void TestTurnOrderThreeHeroesOblivaeonFirst()
        {
            SetupGameController(
                "OblivAeon",
                "Jp.ParahumansOfTheWormverse.Labyrinth", "Legacy", "Tachyon",
                "InsulaPrimalis", "Magmaria", "MobileDefensePlatform", "RookCity", "TheFinalWasteland");

            StartGame();

            foreach (var c in oblivaeon.CharacterCards) { FindCardController(c).RemoveAllTriggers(); }
            foreach (var c in scionOne.CharacterCards) { FindCardController(c).RemoveAllTriggers(); }
            foreach (var c in scionTwo.CharacterCards) { FindCardController(c).RemoveAllTriggers(); }

            MoveAllCards(envOne, envOne.TurnTaker.Deck, envOne.TurnTaker.OutOfGame);
            MoveAllCards(envTwo, envTwo.TurnTaker.Deck, envTwo.TurnTaker.OutOfGame);
            MoveAllCards(oblivaeon, oblivaeon.TurnTaker.Deck, oblivaeon.TurnTaker.OutOfGame);
            MoveAllCards(scionOne, scionOne.TurnTaker.Deck, scionOne.TurnTaker.OutOfGame);
            MoveAllCards(scionTwo, scionTwo.TurnTaker.Deck, scionTwo.TurnTaker.OutOfGame);


            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(oblivaeon.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(legacy.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(tachyon.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(envOne.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(labyrinth.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(scionOne.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(envTwo.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(scionTwo.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(oblivaeon.TurnTaker));
        }

        [Test()]
        public void TestTurnOrderThreeHeroesOblivaeonSecond()
        {
            SetupGameController(
                "OblivAeon",
                "Legacy", "Jp.ParahumansOfTheWormverse.Labyrinth", "Tachyon",
                "InsulaPrimalis", "Magmaria", "MobileDefensePlatform", "RookCity", "TheFinalWasteland");

            StartGame();

            foreach (var c in oblivaeon.CharacterCards) { FindCardController(c).RemoveAllTriggers(); }
            foreach (var c in scionOne.CharacterCards) { FindCardController(c).RemoveAllTriggers(); }
            foreach (var c in scionTwo.CharacterCards) { FindCardController(c).RemoveAllTriggers(); }

            MoveAllCards(envOne, envOne.TurnTaker.Deck, envOne.TurnTaker.OutOfGame);
            MoveAllCards(envTwo, envTwo.TurnTaker.Deck, envTwo.TurnTaker.OutOfGame);
            MoveAllCards(oblivaeon, oblivaeon.TurnTaker.Deck, oblivaeon.TurnTaker.OutOfGame);
            MoveAllCards(scionOne, scionOne.TurnTaker.Deck, scionOne.TurnTaker.OutOfGame);
            MoveAllCards(scionTwo, scionTwo.TurnTaker.Deck, scionTwo.TurnTaker.OutOfGame);


            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(oblivaeon.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(legacy.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(tachyon.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(envOne.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(labyrinth.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(scionOne.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(envTwo.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(scionTwo.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(oblivaeon.TurnTaker));
        }

        [Test()]
        public void TestTurnOrderThreeHeroesOblivaeonLast()
        {
            SetupGameController(
                "OblivAeon",
                "Legacy", "Tachyon", "Jp.ParahumansOfTheWormverse.Labyrinth",
                "InsulaPrimalis", "Magmaria", "MobileDefensePlatform", "RookCity", "TheFinalWasteland");

            StartGame();

            foreach (var c in oblivaeon.CharacterCards) { FindCardController(c).RemoveAllTriggers(); }
            foreach (var c in scionOne.CharacterCards) { FindCardController(c).RemoveAllTriggers(); }
            foreach (var c in scionTwo.CharacterCards) { FindCardController(c).RemoveAllTriggers(); }

            MoveAllCards(envOne, envOne.TurnTaker.Deck, envOne.TurnTaker.OutOfGame);
            MoveAllCards(envTwo, envTwo.TurnTaker.Deck, envTwo.TurnTaker.OutOfGame);
            MoveAllCards(oblivaeon, oblivaeon.TurnTaker.Deck, oblivaeon.TurnTaker.OutOfGame);
            MoveAllCards(scionOne, scionOne.TurnTaker.Deck, scionOne.TurnTaker.OutOfGame);
            MoveAllCards(scionTwo, scionTwo.TurnTaker.Deck, scionTwo.TurnTaker.OutOfGame);


            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(oblivaeon.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(legacy.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(tachyon.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(envOne.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(labyrinth.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(scionOne.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(envTwo.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(scionTwo.TurnTaker));
            GoToNextTurn();
            Assert.That(GameController.ActiveTurnTaker, Is.EqualTo(oblivaeon.TurnTaker));
        }
    }
}
