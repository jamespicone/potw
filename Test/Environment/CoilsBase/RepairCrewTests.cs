using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Environment.CoilsBase
{
    [TestFixture()]
    public class RepairCrewTests : CoilsBaseTestBase
    {
        [Test()]
        public void TestStructuresRegainHPAtEndOfTurn()
        {
            SetupCoilsBaseGame();

            var crew = PlayCard("RepairCrew");
            var cladding = PlayCard("AblativeCladding");

            SetHitPoints(cladding, 3);
            SetHitPoints(crew, 3);

            GoToEndOfTurn(env);

            // The structure heals 3; the (non-structure) crew doesn't.
            AssertHitPoints(cladding, 6);
            AssertHitPoints(crew, 3);
        }
    }
}
