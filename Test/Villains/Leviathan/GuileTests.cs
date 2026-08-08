using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Leviathan
{
    [TestFixture()]
    public class GuileTests : LeviathanTestBase
    {
        [Test()]
        public void TestFlipsLeviathan()
        {
            SetupLeviathanGame();
            MoveTacticsToDeckBottom();

            PlayCard("Guile");

            AssertFlipped(leviathan.CharacterCard);
        }

        [Test()]
        public void TestFlipPutsTacticIntoPlay()
        {
            SetupLeviathanGame();
            MoveTacticsToDeckBottom();
            var downpour = MoveTacticToDeckTop("TorrentialDownpour");

            PlayCard("Guile");

            AssertFlipped(leviathan.CharacterCard);
            AssertIsInPlay(downpour);
        }
    }
}
