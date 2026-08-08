using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.JessicaYamada
{
    [TestFixture()]
    public class AHelpfulSuggestionTests : ParahumanTest
    {
        [Test()]
        public void TestOtherPlayerMayPlayACard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Legacy", "Megalopolis");
            StartGame();
            RemoveVillainCards();
            RemoveVillainTriggers();

            var fortitude = PutInHand("Fortitude");
            DecisionSelectCard = fortitude;

            PlayCard("AHelpfulSuggestion", 0);

            AssertIsInPlay(fortitude);
        }
    }
}
