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
    public class ChimaericalNightmareTests : ParahumanTest
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

            PlayCard("ChimaericalNightmare");

            var field = PlayCard("ObsidianField");

            var decider = AssertNoDecision();
            var deckCardCount = echidna.TurnTaker.Deck.NumberOfCards;
            DestroyCard(field);
            RestoreOnMakeDecisions(decider);

            AssertNumberOfCardsInDeck(echidna, deckCardCount);

            var raptor = PlayCard("VelociraptorPack");
            deckCardCount = echidna.TurnTaker.Deck.NumberOfCards;
            DestroyCard(raptor);
            var currentCount = echidna.TurnTaker.Deck.NumberOfCards;
            Assert.Less(currentCount, deckCardCount);
        }
    }
}
