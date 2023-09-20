﻿using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Battery
{
    [TestFixture()]
    public class RandomBatteryCauldronCapeTests : RandomParahumanTest
    {
        private string HeroToUse = "Jp.ParahumansOfTheWormverse.Battery";
        private string Promo = "BatteryCauldronCapeCharacter";

        [Test]
        [Category("Random")]
        public void TestRandom()
        {
            var gc = SetupRandomParahumanTest(
                useHeroes: new List<string>{ HeroToUse },
                overrideVariants: new Dictionary<string, string> { { HeroToUse, Promo } }
            );
            RunGame(gc);
        }

        [Test]
        [Category("Random")]
        public void TestRandomWithLabyrinth()
        {
            var gc = SetupRandomParahumanTestWithLabyrinth(
                useHeroes: new List<string> { HeroToUse },
                overrideVariants: new Dictionary<string, string> { { HeroToUse, Promo } }
            );
            RunGame(gc);
        }

        [Test]
        [Category("Random")]
        public void TestEnvRandomWithPWT()
        {
            var gc = SetupRandomParahumanTestWithPWTempest(
                useHeroes: new List<string> { HeroToUse },
                overrideVariants: new Dictionary<string, string> { { HeroToUse, Promo } }
            );
            RunGame(gc);
        }

        [Test]
        [Category("Random")]
        public void TestEnvRandomWithGuise()
        {
            var gc = SetupRandomParahumanTestWithGuise(
                useHeroes: new List<string> { HeroToUse },
                overrideVariants: new Dictionary<string, string> { { HeroToUse, Promo } }
            );
            RunGame(gc);
        }

        [Test]
        [Category("Random")]
        public void TestEnvRandomWithCompletionistGuise()
        {
            var gc = SetupRandomParahumanTestWithCompletionistGuise(
                useHeroes: new List<string> { HeroToUse },
                overrideVariants: new Dictionary<string, string> { { HeroToUse, Promo } }
            );
            RunGame(gc);
        }
    }
}
