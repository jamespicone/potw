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
    public class StrategyAndSuggestionTests : ParahumanTest
    {
        [Test()]
        public void TestRevealsUntilOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Legacy", "Megalopolis");
            StartGame();
            RemoveVillainCards();
            RemoveVillainTriggers();

            // Top of Legacy's deck: Thokk (one-shot), then Fortitude (ongoing).
            var fortitude = StackDeck(legacy, "Fortitude");
            var thokk = StackDeck(legacy, "Thokk");

            PlayCard("StrategyAndSuggestion", 0);

            // The revealed ongoing was put into play (the default choice); the
            // one-shot was shuffled back.
            AssertIsInPlay(fortitude);
            AssertInDeck(thokk);
        }
    }
}
