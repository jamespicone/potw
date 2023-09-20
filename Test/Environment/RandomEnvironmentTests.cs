using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Environment
{
    [TestFixture()]
    [TestFixtureSource(nameof(RandomEnvironments))]
    public class _RandomEnvironmentTests : RandomParahumanTest
    {
        static TestFixtureData TestData(string env, string name)
        {
            var ret = new TestFixtureData("Jp.ParahumansOfTheWormverse." + env);
            ret.TestName = name + " Random Tests";
            return ret;
        }

        private static IEnumerable RandomEnvironments()
        {
            yield return TestData("BrocktonBay", "Brockton Bay");
            yield return TestData("CoilsBase", "Coil's Base");
            yield return TestData("Kyushu", "Kyushu");
            yield return TestData("NewDelhi", "New Delhi");
        }

        public _RandomEnvironmentTests(string env)
        {
            EnvToUse = env;
        }

        private string EnvToUse = "";

        [Test]
        [Category("Random")]
        public void TestEnvRandom()
        {
            var gc = SetupRandomParahumanTest(overrideEnvironment: EnvToUse);
            RunGame(gc);
        }

        [Test]
        [Category("Random")]
        public void TestEnvRandomWithLabyrinth()
        {
            var gc = SetupRandomParahumanTestWithLabyrinth(overrideEnvironment: EnvToUse);
            RunGame(gc);
        }

        [Test]
        [Category("Random")]
        public void TestEnvRandomWithPWT()
        {
            var gc = SetupRandomParahumanTestWithPWTempest(overrideEnvironment: EnvToUse);
            RunGame(gc);
        }
    }
}
