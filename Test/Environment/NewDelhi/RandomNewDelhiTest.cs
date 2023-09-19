using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.BrocktonBay
{
    [TestFixture()]
    public class RandomNewDelhiTests : RandomParahumanTest
    {
        [Test]
        [Property("TestType", "Random")]
        public void TestEnvRandom()
        {
            var gc = SetupRandomParahumanTest(overrideEnvironment: "Jp.ParahumansOfTheWormverse.NewDelhi");
            RunGame(gc);
        }

        [Test]
        [Property("TestType", "Random")]
        public void TestEnvRandomWithLabyrinth()
        {
            var gc = SetupRandomParahumanTestWithLabyrinth(overrideEnvironment: "Jp.ParahumansOfTheWormverse.NewDelhi");
            RunGame(gc);
        }

        [Test]
        [Property("TestType", "Random")]
        public void TestEnvRandomWithPWT()
        {
            var gc = SetupRandomParahumanTestWithPWTempest(overrideEnvironment: "Jp.ParahumansOfTheWormverse.NewDelhi");
            RunGame(gc);
        }
    }
}
