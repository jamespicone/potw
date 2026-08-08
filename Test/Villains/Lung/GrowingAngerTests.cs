using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Lung
{
    [TestFixture()]
    public class GrowingAngerTests : LungTestBase
    {
        [Test()]
        public void TestDiscardsThreeAndPlaysTopCard()
        {
            SetupLungGame();
            RemoveLungTriggers();

            // Stack order after these calls (top first): Smash, Smash, Smash, Wings
            var stacked = StackDeckHandleDuplicates("Wings", "Smash", "Smash", "Smash").ToList();
            var wings = stacked[0];
            var smashes = stacked.Skip(1).ToList();

            QuickHPStorage(legacy, bunker, haka);
            AssertWillBeDiscarded(smashes.ToList());

            PlayCard("GrowingAnger");

            // The three Smashes were discarded, not played...
            AssertInTrash(smashes[0]);
            AssertInTrash(smashes[1]);
            AssertInTrash(smashes[2]);
            AssertAllDiscardsDiscarded();
            QuickHPCheck(0, 0, 0);

            // ... and the card underneath was played.
            AssertIsInPlay(wings);
        }
    }
}
