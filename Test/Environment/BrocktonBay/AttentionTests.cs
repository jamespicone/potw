using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Environment.BrocktonBay
{
    [TestFixture()]
    public class AttentionTests : BrocktonBayTestBase
    {
        [Test()]
        public void TestRevealsTargetsIntoPlayAndSelfDestructs()
        {
            SetupBrocktonBayGame();

            // Villain deck top is a target; environment deck top is a target;
            // hero decks contain no targets and get shuffled back.
            var battalion = StackDeck(baron, "BladeBattalion");
            var squad = StackDeck("PRTSquad");

            var attention = PlayCard("Attention");

            GoToEndOfTurn(env);

            AssertIsInPlay(battalion);
            AssertIsInPlay(squad);
            AssertInTrash(attention);
        }
    }
}
