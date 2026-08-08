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
    public class ConstructionWorkTests : CoilsBaseTestBase
    {
        [Test()]
        public void TestPutsThreeStructuresIntoPlayAndSelfDestructs()
        {
            SetupCoilsBaseGame();

            var construction = PlayCard("ConstructionWork");

            GoToEndOfTurn(env);

            Assert.That(
                FindCardsWhere(c => c.DoKeywordsContain("structure") && c.IsInPlay).Count(),
                Is.EqualTo(3));
            AssertInTrash(construction);
        }
    }
}
