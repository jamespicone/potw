﻿using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Hero
{
    [TestFixture()]
    [TestFixtureSource(nameof(RandomHeroes))]
    public class _RandomHeroTests : RandomParahumanTest
    {
        static TestFixtureData TestData(string hero, string variant, string name)
        {
            var ret = new TestFixtureData("Jp.ParahumansOfTheWormverse." + hero, variant);
            ret.TestName = name + " Random Tests";
            return ret;
        }

        private static IEnumerable RandomHeroes()
        {
            yield return TestData("Alexandria", "AlexandriaCharacter", "Alexandria");
            yield return TestData("Battery", "BatteryCharacter", "Battery");
            yield return TestData("Battery", "BatteryCauldronCapeCharacter", "BatteryCauldronCape");
            yield return TestData("Bitch", "BitchCharacter", "Bitch");
            yield return TestData("Dauntless", "DauntlessCharacter", "Dauntless");
            yield return TestData("Dragon", "DragonCharacter", "Dragon");
            yield return TestData("Grue", "GrueCharacter", "Grue");
            yield return TestData("JessicaYamada", "JessicaYamadaInstructionsTarget", "JessicaTarget");
            yield return TestData("JessicaYamada", "JessicaYamadaInstructionsEnvironment", "JessicaEnv");
            yield return TestData("JessicaYamada", "JessicaYamadaInstructionsNotTarget", "JessicaNotTarget");
            yield return TestData("Labyrinth", "LabyrinthCharacter", "Labyrinth");
            yield return TestData("Legend", "LegendCharacter", "Legend");
            yield return TestData("MissMilitia", "MissMilitiaCharacter", "Miss Militia");
            yield return TestData("MissMilitia", "MissMilitiaProtectorateCaptainCharacter", "Miss Militia Protectorate Captain");
            yield return TestData("Skitter", "SkitterCharacter", "Skitter");
            yield return TestData("Tattletale", "TattletaleCharacter", "Tattletale");
            yield return TestData("Tattletale", "TattletaleRulerOfBrocktonBayCharacter", "Tattletale Ruler");
            yield return TestData("Tattletale", "TattletaleHunterOfSecretsCharacter", "Tattletale Hunter");
        }

        public _RandomHeroTests(string hero, string variant)
        {
            HeroToUse = hero;
            VariantToUse = variant;
        }

        private string HeroToUse = "";
        private string VariantToUse = "";

        [Test]
        [Category("Random")]
        public void TestRandom()
        {
            var gc = SetupRandomParahumanTest(
                useHeroes: new List<string> { HeroToUse },
                overrideVariants: new Dictionary<string, string> { { HeroToUse, VariantToUse } }
            );
            RunGame(gc);
        }

        [Test]
        [Category("Random")]
        public void TestRandomWithLabyrinth()
        {
            var gc = SetupRandomParahumanTestWithLabyrinth(
                useHeroes: new List<string> { HeroToUse },
                overrideVariants: new Dictionary<string, string> { { HeroToUse, VariantToUse } }
            );
            RunGame(gc);
        }

        [Test]
        [Category("Random")]
        public void TestEnvRandomWithPWT()
        {
            var gc = SetupRandomParahumanTestWithPWTempest(
                useHeroes: new List<string> { HeroToUse },
                overrideVariants: new Dictionary<string, string> { { HeroToUse, VariantToUse } }
            );
            RunGame(gc);
        }

        [Test]
        [Category("Random")]
        public void TestEnvRandomWithGuise()
        {
            var gc = SetupRandomParahumanTestWithGuise(
                useHeroes: new List<string> { HeroToUse },
                overrideVariants: new Dictionary<string, string> { { HeroToUse, VariantToUse } }
            );
            RunGame(gc);
        }

        [Test]
        [Category("Random")]
        public void TestEnvRandomWithCompletionistGuise()
        {
            var gc = SetupRandomParahumanTestWithCompletionistGuise(
                useHeroes: new List<string> { HeroToUse },
                overrideVariants: new Dictionary<string, string> { { HeroToUse, VariantToUse } }
            );
            RunGame(gc);
        }

        [Test]
        [Category("Random")]
        public void TestEnvRandomWithTribunal()
        {
            var gc = SetupRandomParahumanTestWithTribunal(
                useHeroes: new List<string> { HeroToUse },
                overrideVariants: new Dictionary<string, string> { { HeroToUse, VariantToUse } }
            );
            RunGame(gc);
        }
    }
}
