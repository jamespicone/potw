using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Environment.NewDelhi
{
    [TestFixture()]
    public class AccordsPlanTests : NewDelhiTestBase
    {
        [Test()]
        public void TestRevealsAndPlaysOngoingsEquipmentDevices()
        {
            SetupNewDelhiGame();

            // Villain top is an ongoing (played), Legacy's top is a one-shot
            // (shuffled back), Bunker's top is equipment (played).
            var backlash = StackDeck(baron, "BacklashField");
            var thokk = StackDeck(legacy, "Thokk");
            var flak = StackDeck(bunker, "FlakCannon");

            var plan = PlayCard("AccordsPlan");

            AssertIsInPlay(backlash);
            AssertIsInPlay(flak);
            AssertInDeck(thokk);
            AssertNotInPlay(thokk);

            // At the end of the environment turn it destroys itself.
            GoToEndOfTurn(env);
            AssertInTrash(plan);
        }
    }
}
