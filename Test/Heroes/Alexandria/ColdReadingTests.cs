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
    public class ColdReadingTests : ParahumanTest
    {
        [Test()]
        public void TestRevealOnPlay()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            StackDeck("BacklashField", "ElementalRedistributor", "PoweredRemoteTurret");


            AssertRevealed();

            PlayCard("ColdReading");
        }

        [Test()]
        public void TestStartOfTurnMayDiscardTopVillainCard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            PlayCard("ColdReading");

            var top = StackDeck(baron, "BacklashField");
            DecisionMoveCardDestination = new MoveCardDestination(baron.TurnTaker.Trash);

            GoToStartOfTurn(alexandria);

            AssertInTrash(top);
        }

        [Test()]
        public void TestStartOfTurnMayReturnTopVillainCard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            PlayCard("ColdReading");

            var top = StackDeck(baron, "BacklashField");
            DecisionMoveCardDestination = new MoveCardDestination(baron.TurnTaker.Deck);

            GoToStartOfTurn(alexandria);

            AssertOnTopOfDeck(baron, top);
        }
    }
}
