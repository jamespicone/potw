using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Jp.ParahumansOfTheWormverse.Labyrinth;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.CodeDom;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Labyrinth
{
    [TestFixture()]
    public class ExplorationTests : ParahumanTest
    {
        [Test()]
        public void TestPlay()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Legacy", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();

            MoveAllCardsFromHandToDeck(labyrinth);

            var cards = StackDeck("ObsidianField", "VelociraptorPack");
            var field = cards.First();
            var raptor = cards.Skip(1).First();

            var inspiration = StackDeck("Inspiration");

            QuickHandStorage(labyrinth);

            PlayCard("Exploration");

            QuickHandCheck(1);

            AssertInHand(inspiration);
            AssertUnderCard(labyrinth.CharacterCard, raptor);
            AssertIsInPlay(field);
        }
    }
}
