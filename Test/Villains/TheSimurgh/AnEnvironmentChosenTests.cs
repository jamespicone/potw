using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.TheSimurgh
{
    [TestFixture()]
    public class AnEnvironmentChosenTests : SimurghTestBase
    {
        [Test()]
        public void TestPlaysTopEnvironmentCards()
        {
            SetupSimurghGame();
            RemoveSimurghTriggers();

            var hostage = StackDeck("HostageSituation");
            var paparazzi = StackDeck("PaparazziOnTheScene");
            var pileup = StackDeck("TrafficPileup");

            PlayCard("AnEnvironmentChosen");

            // Plays the top H = 3 cards of the environment deck.
            AssertIsInPlay(hostage);
            AssertIsInPlay(paparazzi);
            AssertIsInPlay(pileup);
        }
    }
}
