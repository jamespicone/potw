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
    public class DevastationTests : NewDelhiTestBase
    {
        [Test()]
        public void TestDestroysAllNonCharacterCards()
        {
            SetupNewDelhiGame();

            var fortitude = PlayCard("Fortitude");
            var rod = PlayCard("LightningRod");

            var devastation = PlayCard("Devastation");

            AssertInTrash(fortitude);
            AssertInTrash(rod);
            AssertIsInPlay(devastation);
        }

        [Test()]
        public void TestSelfDestructsAtEndOfTurn()
        {
            SetupNewDelhiGame();

            var devastation = PlayCard("Devastation");

            GoToEndOfTurn(env);

            AssertInTrash(devastation);
        }
    }
}
