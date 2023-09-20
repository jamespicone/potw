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
    [TestFixture(Category = "RandomCategory")]
    [TestFixtureSource(nameof(RandomEnvironments))]
    public class RandomEnvironmentTests : RandomParahumanTest
    {
        static TestFixtureData TestData(string env)
        {
            var identifier = "Jp.ParahumansOfTheWormverse." + env;
            var ret = new TestFixtureData(identifier);
            var definition = DeckDefinitionCache.GetDeckDefinition(identifier);

            ret.TestName = definition.Name + " Random Tests";
            return ret;
        }

        private static IEnumerable RandomEnvironments()
        {
            yield return TestData("BrocktonBay");
            yield return TestData("CoilsBase");
            yield return TestData("Kyushu");
            yield return TestData("NewDelhi");
        }

        public RandomEnvironmentTests(string env)
        {
            EnvToUse = env;
        }

        private string EnvToUse = "";

        [Test]
        [Property("TestType", "Random")]
        public void TestEnvRandom()
        {
            var gc = SetupRandomParahumanTest(overrideEnvironment: EnvToUse);
            RunGame(gc);
        }

        [Test]
        [Property("TestType", "Random")]
        public void TestEnvRandomWithLabyrinth()
        {
            var gc = SetupRandomParahumanTestWithLabyrinth(overrideEnvironment: EnvToUse);
            RunGame(gc);
        }

        [Test]
        [Property("TestType", "Random")]
        public void TestEnvRandomWithPWT()
        {
            var gc = SetupRandomParahumanTestWithPWTempest(overrideEnvironment: EnvToUse);
            RunGame(gc);
        }
    }
}
