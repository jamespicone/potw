using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Villain
{
    [TestFixture(Category = "RandomCategory")]
    [TestFixtureSource(nameof(RandomVillains))]
    public class RandomVillainTests : RandomParahumanTest
    {
        static TestFixtureData TestData(string villain, string variant, string name)
        {
            var ret = new TestFixtureData("Jp.ParahumansOfTheWormverse." + villain, variant);
            ret.TestName = name + " Random Tests";
            return ret;
        }

        private static IEnumerable RandomVillains()
        {
            yield return TestData("Behemoth", "BehemothCharacter", "Behemoth");
            yield return TestData("Coil", "CoilCharacter", "Coil");
            yield return TestData("Echidna", "EchidnaCharacter", "Echidna");
            yield return TestData("Leviathan", "LeviathanCharacter", "Leviathan");
            yield return TestData("Lung", "LungCharacter", "Lung");
            yield return TestData("Slaughterhouse9", "Slaughterhouse9Character", "Slaughterhouse 9");
            yield return TestData("TheMerchants", "SkidmarkCharacter", "Merchants");
            yield return TestData("TheSimurgh", "TheSimurghCharacter", "Simurgh");
        }



        public RandomVillainTests(string villain, string variant)
        {
            VillainToUse = villain;
            VariantToUse = variant;
        }

        private string VillainToUse = "";
        private string VariantToUse = "";

        [Test]
        [Property("TestType", "Random")]
        public void TestRandom()
        {
            var gc = SetupRandomParahumanTest(
                overrideVillain: VillainToUse,
                overrideVariants: new Dictionary<string, string> { { VillainToUse, VariantToUse } }
            );
            RunGame(gc);
        }

        [Test]
        [Property("TestType", "Random")]
        public void TestRandomWithLabyrinth()
        {
            var gc = SetupRandomParahumanTestWithLabyrinth(
                overrideVillain: VillainToUse,
                overrideVariants: new Dictionary<string, string> { { VillainToUse, VariantToUse } }
            );
            RunGame(gc);
        }

        [Test]
        [Property("TestType", "Random")]
        public void TestRandomWithPWTempest()
        {
            var gc = SetupRandomParahumanTestWithPWTempest(
                overrideVillain: VillainToUse,
                overrideVariants: new Dictionary<string, string> { { VillainToUse, VariantToUse } }
            );
            RunGame(gc);
        }

        [Test]
        [Property("TestType", "Random")]
        public void TestRandomWithTribunal()
        {
            var gc = SetupRandomParahumanTestWithTribunal(
                overrideVillain: VillainToUse,
                overrideVariants: new Dictionary<string, string> { { VillainToUse, VariantToUse } }
            );
            RunGame(gc);
        }
    }
}
