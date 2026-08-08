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
    public class CloneArmyTests : ParahumanTest
    {
        [Test()]
        public void TestWorks()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "InsulaPrimalis"
            );

            StartGame();
            ReturnAllTwisted();

            PlayCard("CloneArmy");
            StackDeck("PandemicTwisted");
            StackDeck("Crush");

            AssertNotInPlay("PandemicTwisted");

            GoToEndOfTurn(echidna);

            AssertIsInPlay("PandemicTwisted");
        }

        [Test()]
        public void TestDoesNotTriggerAtTheEndOfAHeroTurn()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "InsulaPrimalis"
            );

            StartGame();
            ReturnAllTwisted();

            PlayCard("CloneArmy");
            StackDeck("Crush");

            // The Twisted deck is empty here, so this also checks the
            // end-of-villain-turn play copes with having nothing to play.
            GoToEndOfTurn(echidna);

            StackDeck("PandemicTwisted");

            GoToEndOfTurn(alexandria);

            AssertNotInPlay("PandemicTwisted");
        }
    }
}
